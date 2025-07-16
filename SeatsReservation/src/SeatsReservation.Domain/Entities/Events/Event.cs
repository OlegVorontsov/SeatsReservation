using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Events.EventInfo;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Entities.Events;

public class Event
{
    public Id<Event> Id { get; set; } = null!;

    public string Name { get; private set; }
    
    public DateTimeOffset EventDate { get; private set; }
    
    public EventDetails Details { get; private set; } = null!;
    
    public Id<Venue> VenueId { get; set; }
    
    public EventType EventType { get; private set; }
    
    public IEventInfo EventInfo { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset EndedAt { get; private set; }
    public EventStatus Status { get; private set; }
    
    //Ef Core
    private Event() { }

    private Event(
        Id<Event> id,
        Id<Venue> venueId,
        string name,
        DateTimeOffset eventDate,
        DateTimeOffset startedAt,
        DateTimeOffset endedAt,
        EventDetails details,
        EventType eventType,
        IEventInfo eventInfo)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        StartedAt = startedAt;
        EndedAt = endedAt;
        Status = EventStatus.Planned;
        Details = details;
        EventType = eventType;
        EventInfo = eventInfo;
    }

    public bool IsAvailableForReservation(int capacitySum) =>
        Status == EventStatus.Planned &&
        StartedAt > DateTimeOffset.UtcNow &&
        capacitySum <= Details.Capacity;

    private static Result<EventDetails, Error> Validate(
        string name,
        DateTimeOffset eventDate,
        DateTimeOffset startedAt,
        DateTimeOffset endedAt,
        int capacity,
        string description)
    {
        if (startedAt >= endedAt || startedAt <= DateTimeOffset.UtcNow || endedAt <= DateTimeOffset.UtcNow)
            return Error.Validation("event.time", "Event time incorrect");
        
        if(string.IsNullOrWhiteSpace(name))
            return Error.Validation("event.name", "Event name is required");
        
        if(eventDate < DateTimeOffset.UtcNow)
            return Error.Validation("event.date", "Event date cannot be in the past");
        
        if(capacity <= 0)
            return Error.Validation("event.capacity", "Event capacity must be greater than zero");
        
        if(string.IsNullOrWhiteSpace(description))
            return Error.Validation("event.description", "Event description is required");
        
        return new EventDetails(capacity, description);
    }

    public static Result<Event, Error> Create(
        EventType eventType,
        Id<Venue> venueId,
        string name,
        DateTimeOffset eventDate,
        DateTimeOffset startedAt,
        DateTimeOffset endedAt,
        int capacity,
        string description,
        string? performer,
        string? speaker,
        string? topic,
        string? url)
    {
        var detailsResult = Validate(name, eventDate, startedAt, endedAt, capacity, description);
        if (detailsResult.IsFailure)
            return detailsResult.Error;
        
        switch (eventType)
        {
            case EventType.Concert:
                if(string.IsNullOrWhiteSpace(performer))
                    return Error.Validation("event.performer", "Event performer is required");
                var concertInfo = new ConcertInfo(performer);
                
                return new Event(
                    Id<Event>.Create(Guid.NewGuid()),
                    venueId,
                    name,
                    eventDate,
                    startedAt,
                    endedAt,
                    detailsResult.Value,
                    EventType.Concert,
                    concertInfo);

            case EventType.Conference:
                if(string.IsNullOrWhiteSpace(speaker))
                    return Error.Validation("event.speaker", "Event speaker is required");
                if(string.IsNullOrWhiteSpace(topic))
                    return Error.Validation("event.topic", "Event topic is required");
                var conferenceInfo = new ConferenceInfo(speaker, topic);
                
                return new Event(
                    Id<Event>.Create(Guid.NewGuid()),
                    venueId,
                    name,
                    eventDate,
                    startedAt,
                    endedAt,
                    detailsResult.Value,
                    EventType.Conference,
                    conferenceInfo);
            
            case EventType.Online:
                if(string.IsNullOrWhiteSpace(url))
                    return Error.Validation("event.url", "Event url is required");
                var onlineInfo = new OnlineInfo(url);
                
                return new Event(
                    Id<Event>.Create(Guid.NewGuid()),
                    venueId,
                    name,
                    eventDate,
                    startedAt,
                    endedAt,
                    detailsResult.Value,
                    EventType.Online,
                    onlineInfo);
        }
        return Error.Failure("create.event", "Failed to create event");
    }
}

public enum EventStatus
{
    Planned,
    InProgress,
    Finished,
    Cancelled
}