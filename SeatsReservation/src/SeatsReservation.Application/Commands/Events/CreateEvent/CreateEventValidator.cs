using FluentValidation;
using SeatsReservation.Domain.Entities.Events;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Events.CreateEvent;

public class CreateEventValidator :
    AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("Name"));
        
        RuleFor(c => c.EventDate)
            .NotNull()
            .NotEmpty()
            .Must(date => date >= DateTimeOffset.UtcNow)
            .WithError(Errors.General.ValueIsRequired("EventDate"));
        
        RuleFor(c => c.Capacity)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsRequired("Capacity"));
        
        RuleFor(c => c.VenueId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("VenueId"));
        
        RuleFor(c => c.EventType)
            .IsEnumName(typeof(EventType))
            .WithError(Errors.General.ValueIsInvalid("EventType"));
        
        RuleFor(c => c.StartedAt)
            .NotNull()
            .NotEmpty()
            .Must(date => date >= DateTimeOffset.UtcNow)
            .WithError(Errors.General.ValueIsRequired("StartedAt"));
        
        RuleFor(c => c.EndedAt)
            .NotNull()
            .NotEmpty()
            .Must(date => date >= DateTimeOffset.UtcNow)
            .WithError(Errors.General.ValueIsRequired("EndedAt"));
    }
}