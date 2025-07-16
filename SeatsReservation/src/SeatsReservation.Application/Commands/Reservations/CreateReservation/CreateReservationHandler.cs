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
    ISeatsRepository seatsRepository,
    ITransactionManager transactionManager)
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
        
        var transactionScopeResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;
        
        var eventResult = await eventsRepository.GetById(
            eventId, cancellationToken);
        if (eventResult.IsFailure)
        {
            transactionScope.Rollback();
            return eventResult.Error.ToErrors();
        }
        
        var reservedSeatsCount = await reservationsRepository.GetReservedSeatsCount(eventId, cancellationToken);

        if (eventResult.Value.IsAvailableForReservation(reservedSeatsCount + command.SeatsIds.Count()) == false)
        {
            transactionScope.Rollback();
            return Error.Failure("reservation.fail", "Reservation is too large").ToErrors();
        }
        
        var seatIds = command.SeatsIds.Select(Id<Seat>.Create).ToList();
        var seats = await seatsRepository.GetByIds(seatIds, cancellationToken);
        if (seats.Any(seat => seat.VenueId != eventResult.Value.VenueId) || seats.Count == 0)
        {
            transactionScope.Rollback();
            return Error.Conflict("seats.conflict", "Seats doesn't belong to venue").ToErrors();
        }
        
        // проверка не нужна тк был добавлен индекс в ReservationSeatConfiguration
        /*var isSeatsReserved = await reservationsRepository
            .IsAnySeatsReserved(eventId, seatIds, cancellationToken);
        if (isSeatsReserved)
            return Error.Conflict("seats.conflict", "Seats already reserved").ToErrors();*/
        
        var reservationResult = Reservation.Create(
            command.EventId, command.UserId, command.SeatsIds);
        if (reservationResult.IsFailure)
        {
            transactionScope.Rollback();
            return reservationResult.Error.ToErrors();
        }
        
        var createResult = await reservationsRepository.CreateAsync(reservationResult.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            transactionScope.Rollback();
            return createResult.Error.ToErrors();
        }

        var commitedResult = transactionScope.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();
        
        return ReservationDto.FromDomainEntity(reservationResult.Value);
    }
}