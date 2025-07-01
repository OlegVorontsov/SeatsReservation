using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.CreateVenue;

public class CreateVenueValidator :
    AbstractValidator<CreateVenueCommand>
{
    public CreateVenueValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Name"));
        
        RuleFor(c => c.SeatsLimit)
            .ExclusiveBetween(0, 10000)
            .WithError(Errors.General.ValueIsInvalid("SeatsLimit"));
        
        RuleFor(c => c.Seats)
            .Must(seats => seats.All(s => s.SeatNumber > 0 && s.SeatNumber <= 1000))
            .WithError(Errors.General.ValueIsInvalid("SeatNumber"));
        
        RuleFor(c => c.Seats)
            .Must(seats => seats.All(s => s.RowNumber > 0 && s.RowNumber <= 1000))
            .WithError(Errors.General.ValueIsInvalid("RowNumber"));
    }
}