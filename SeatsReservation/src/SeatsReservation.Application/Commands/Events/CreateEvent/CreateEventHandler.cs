using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Commands.Venues;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Events.CreateEvent;

public class CreateEventHandler(
    IValidator<CreateEventCommand> validator,
    IVenuesRepository venuesRepository,
    IEventsRepository eventsRepository)
    : ICommandHandler<EventDto, CreateEventCommand>
{
    public async Task<Result<EventDto, ErrorList>> Handle(
        CreateEventCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        if (!Enum.TryParse<EventType>(command.EventType, out var eventTypeResult))
            return Error.Failure("create.event", "Unknown event type").ToErrors();

        var venueId = Id<Venue>.Create(command.VenueId);
        var venueResult = await venuesRepository.GetById(venueId, cancellationToken);
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();

        var eventResult = Event.Create(
            eventTypeResult,
            venueId,
            command.Name,
            command.EventDate,
            command.StartedAt,
            command.EndedAt,
            command.Capacity,
            command.EventDescription,
            command.Performer,
            command.Speaker,
            command.Topic,
            command.Url);
        if (eventResult.IsFailure)
            return eventResult.Error.ToErrors();
        
        await eventsRepository.CreateAsync(eventResult.Value, cancellationToken);
        
        return EventDto.FromDomainEntity(eventResult.Value);
    }
}