using FluentValidation;
using SeatsReservation.Application.Commands.Reservations.CreateReservation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations.ReserveAdjacentSeats;

public class ReserveAdjacentSeatsValidator :
    AbstractValidator<ReserveAdjacentSeatsCommand>
{
    public ReserveAdjacentSeatsValidator()
    {
        RuleFor(c => c.EventId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("EventId"));
        
        RuleFor(c => c.UserId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("UserId"));
        
        RuleFor(c => c.VenueId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("VenueId"));
        
        RuleFor(c => c.RequiredSeatsCount)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsRequired("RequiredSeatsCount"));
        
        RuleFor(c => c.PreferredRowNumber)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsRequired("PreferredRowNumber"));
    }
}