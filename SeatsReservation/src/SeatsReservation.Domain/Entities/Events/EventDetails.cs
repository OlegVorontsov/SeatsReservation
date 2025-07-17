using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Domain.Entities.Events;

public class EventDetails
{
    public Id<Event> EventId { get; }
    public int Capacity { get; private set; }
    
    public string? Description { get;  private set; }
    
    public DateTimeOffset? LastReservation { get; private set; }
    
    public uint Version { get; private set; }
    
    //Ef Core
    private EventDetails() { }

    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }
    
    public void ReserveSeat() => LastReservation = DateTimeOffset.UtcNow;
}