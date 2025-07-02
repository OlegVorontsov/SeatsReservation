using SeatsReservation.Domain.Entities.Events.EventInfo;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

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
    
    //Ef Core
    private Event() { }

    public Event(Id<Event> id, Id<Venue> venueId, string name, DateTimeOffset eventDate, EventDetails details)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        Details = details;
    }
}