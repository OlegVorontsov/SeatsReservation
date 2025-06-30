namespace SeatsReservation.Domain.Events;

public class Event
{
    public Guid Id { get; set; }

    public string Name { get; private set; }
    
    public DateTimeOffset EventDate { get; private set; }
    
    public EventDetails Details { get; private set; }
    
    public Guid VenueId { get; set; }

    public Event(Guid id, Guid venueId, string name, DateTimeOffset eventDate, EventDetails details)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        Details = details;
    }
}