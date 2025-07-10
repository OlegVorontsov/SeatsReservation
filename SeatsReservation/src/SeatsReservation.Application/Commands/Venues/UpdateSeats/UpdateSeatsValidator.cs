using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateSeats;

public class UpdateSeatsValidator :
    AbstractValidator<UpdateSeatsCommand>
{
    public UpdateSeatsValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Id"));
        
        RuleFor(c => c.Seats)
            .Must(seats => seats.All(s => s.SeatNumber > 0 && s.SeatNumber <= 1000))
            .WithError(Errors.General.ValueIsInvalid("SeatNumber"));
        
        RuleFor(c => c.Seats)
            .Must(seats => seats.All(s => s.RowNumber > 0 && s.RowNumber <= 1000))
            .WithError(Errors.General.ValueIsInvalid("RowNumber"));
    }
}