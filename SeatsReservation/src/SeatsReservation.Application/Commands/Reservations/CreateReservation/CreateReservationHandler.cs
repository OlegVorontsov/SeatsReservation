using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Commands.Events;
using SeatsReservation.Application.Commands.Seats;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations.CreateReservation;

public class CreateReservationHandler(
    IValidator<CreateReservationCommand> validator,
    IReservationsRepository reservationsRepository,
    IEventsRepository eventsRepository,
    ISeatsRepository seatsRepository)
    : ICommandHandler<ReservationDto, CreateReservationCommand>
{
    public async Task<Result<ReservationDto, ErrorList>> Handle(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var eventId = Id<Event>.Create(command.EventId);
        
        var eventResult = await eventsRepository.GetById(
            eventId, cancellationToken);
        if (eventResult.IsFailure)
            return eventResult.Error.ToErrors();
        
        var isAvailable = eventResult.Value.IsAvailableForReservation();
        if (isAvailable == false)
            return Error.Validation("event.not.available", "Event is not available for reservation").ToErrors();
        
        var seatIds = command.SeatsIds.Select(Id<Seat>.Create).ToList();
        var seats = await seatsRepository.GetByIds(seatIds, cancellationToken);
        if (seats.Any(seat => seat.VenueId != eventResult.Value.VenueId) || seats.Count == 0)
            return Error.Conflict("seats.conflict", "Seats doesn't belong to venue").ToErrors();
        
        var isSeatsReserved = await reservationsRepository
            .IsAnySeatsReserved(eventId, seatIds, cancellationToken);
        if (isSeatsReserved)
            return Error.Conflict("seats.conflict", "Seats already reserved").ToErrors();
        
        var reservationResult = Reservation.Create(
            command.EventId, command.UserId, command.SeatsIds);
        if (reservationResult.IsFailure)
            return reservationResult.Error.ToErrors();
        
        await reservationsRepository.CreateAsync(reservationResult.Value, cancellationToken);
        
        return ReservationDto.FromDomainEntity(reservationResult.Value);
    }
}