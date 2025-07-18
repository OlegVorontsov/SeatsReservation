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
        
        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();

        using var transaction = transactionResult.Value;
        
        var eventResult = await eventsRepository.GetByIdWithLock(
            eventId, cancellationToken);
        if (eventResult.IsFailure)
        {
            transaction.Rollback();
            return eventResult.Error.ToErrors();
        }
        
        var reservedSeatsCount = await reservationsRepository.GetReservedSeatsCount(eventId, cancellationToken);

        if (eventResult.Value.IsAvailableForReservation(reservedSeatsCount + command.SeatsIds.Count()) == false)
        {
            transaction.Rollback();
            return Error.Failure("reservation.fail", "Reservation is too large").ToErrors();
        }
        
        var seatIds = command.SeatsIds.Select(Id<Seat>.Create).ToList();
        var seats = await seatsRepository.GetByIds(seatIds, cancellationToken);
        if (seats.Any(seat => seat.VenueId != eventResult.Value.VenueId) || seats.Count == 0)
        {
            transaction.Rollback();
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
            transaction.Rollback();
            return reservationResult.Error.ToErrors();
        }
        
        var createResult = await reservationsRepository.CreateAsync(reservationResult.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            transaction.Rollback();
            return createResult.Error.ToErrors();
        }
        
        // оптимистичная блокировка
        eventResult.Value.Details.ReserveSeat();
        
        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transaction.Rollback();
            return saveResult.Error.ToErrors();
        }

        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();
        
        return ReservationDto.FromDomainEntity(reservationResult.Value);
    }
}