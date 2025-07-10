using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenue;

public class UpdateVenueValidator :
    AbstractValidator<UpdateVenueCommand>
{
    public UpdateVenueValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Id"));
        
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Name"));
        
        RuleFor(c => c.Prefix)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Prefix"));
        
        RuleFor(c => c.SeatsLimit)
            .ExclusiveBetween(0, 10000)
            .WithError(Errors.General.ValueIsInvalid("SeatsLimit"));
    }
}