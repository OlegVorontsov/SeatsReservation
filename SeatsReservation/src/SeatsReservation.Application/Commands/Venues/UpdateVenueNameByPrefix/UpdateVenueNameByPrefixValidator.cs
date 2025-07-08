using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueNameByPrefix;

public class UpdateVenueNameByPrefixValidator :
    AbstractValidator<UpdateVenueNameByPrefixCommand>
{
    public UpdateVenueNameByPrefixValidator()
    {
        RuleFor(c => c.Prefix)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Prefix"));
        
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Name"));
    }
}