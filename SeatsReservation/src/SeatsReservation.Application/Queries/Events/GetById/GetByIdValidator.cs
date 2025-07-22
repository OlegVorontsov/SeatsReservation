using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Queries.Events.GetById;

public class GetByIdValidator:
    AbstractValidator<GetByIdQuery>
{
    public GetByIdValidator()
    {
        RuleFor(d => d.EventId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}