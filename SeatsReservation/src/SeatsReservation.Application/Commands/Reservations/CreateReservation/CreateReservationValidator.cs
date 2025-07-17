using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations.CreateReservation;

public class CreateReservationValidator :
    AbstractValidator<CreateReservationCommand>
{
    public CreateReservationValidator()
    {
        RuleFor(c => c.EventId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("EventId"));
        
        RuleFor(c => c.UserId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("UserId"));
        
        RuleFor(c => c.SeatsIds)
            .ForEach(s => s.NotNull().NotEmpty())
            .WithError(Errors.General.ValueIsInvalid("SeatsIds"));
    }
}