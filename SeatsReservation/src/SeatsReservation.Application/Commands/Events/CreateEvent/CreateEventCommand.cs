using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Events.CreateEvent;

public record CreateEventCommand(
    string Name,
    DateTimeOffset EventDate,
    int Capacity,
    string EventDescription,
    Guid VenueId,
    string EventType,
    string? Performer,
    string? Speaker,
    string? Topic,
    string? Url,
    DateTimeOffset StartedAt,
    DateTimeOffset EndedAt
    ) : ICommand;