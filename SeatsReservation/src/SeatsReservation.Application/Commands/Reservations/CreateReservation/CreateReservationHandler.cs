using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Reservations;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations.CreateReservation;

public class CreateReservationHandler(
    IValidator<CreateReservationCommand> validator,
    IReservationsRepository repository)
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
        
        var reservationResult = Reservation.Create(
            command.EventId, command.UserId, command.SeatsIds);
        if (reservationResult.IsFailure)
            return reservationResult.Error.ToErrors();
    }
}