using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Commands.Events;
using SeatsReservation.Application.Commands.Seats;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.Helpers;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations.ReserveAdjacentSeats;

public class ReserveAdjacentSeatsHandler(
    IValidator<ReserveAdjacentSeatsCommand> validator,
    IReservationsRepository reservationsRepository,
    IEventsRepository eventsRepository,
    ISeatsRepository seatsRepository,
    ITransactionManager transactionManager)
    : ICommandHandler<ReservationDto, ReserveAdjacentSeatsCommand>
{
    public async Task<Result<ReservationDto, ErrorList>> Handle(
        ReserveAdjacentSeatsCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();

        using var transaction = transactionResult.Value;
        
        var eventId = Id<Event>.Create(command.EventId);
        
        var eventResult = await eventsRepository.GetByIdWithLock(eventId, cancellationToken);
        if (eventResult.IsFailure)
        {
            transaction.Rollback();
            return eventResult.Error.ToErrors();
        }
        
        // получаем все доступные места из нужного ряда
        var availableSeats = await seatsRepository.GetAvailableSeats(
            Id<Venue>.Create(command.VenueId),
            eventId, command.PreferredRowNumber, cancellationToken);
        if (availableSeats.Count == 0)
            return Error.NotFound("available.seats", "There are no available seats").ToErrors();
        
        var selectedSeats = command.PreferredRowNumber.HasValue ?
            SeatsHelper.FindAdjacentSeatsInPreferredRow(
                availableSeats, command.RequiredSeatsCount, command.PreferredRowNumber.Value) :
            SeatsHelper.FindBestAdjacentSeats(availableSeats, command.RequiredSeatsCount);
        
        if (selectedSeats.Count == 0)
            return Error.NotFound("selected.seats",
                $"Could not find {command.RequiredSeatsCount} adjacent available seats").ToErrors();
        
        if (selectedSeats.Count < command.RequiredSeatsCount)
            return Error.NotFound("selected.seats",
                $"Only {selectedSeats.Count} adjacent seats available").ToErrors();
        
        var seatIds = selectedSeats.Select(s => s.Id.Value);
        
        var reservationResult = Reservation.Create(
            command.EventId, command.UserId, seatIds);
        if (reservationResult.IsFailure)
        {
            transaction.Rollback();
            return reservationResult.Error.ToErrors();
        }
        
        var createResult = await reservationsRepository.CreateAsync(reservationResult.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            transaction.Rollback();
            return createResult.Error.ToErrors();
        }
        
        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();
        
        return ReservationDto.FromDomainEntity(reservationResult.Value);
    }
}