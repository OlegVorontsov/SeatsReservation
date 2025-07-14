using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueName;

public class UpdateVenueNameValidator :
    AbstractValidator<UpdateVenueNameCommand>
{
    public UpdateVenueNameValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Id"));
        
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Name"));
    }
}