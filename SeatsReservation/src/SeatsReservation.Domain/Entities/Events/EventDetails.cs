namespace SeatsReservation.Domain.Entities.Events;

public class EventDetails
{
    public Guid EventId { get; } = Guid.Empty!;
    
    public int Capacity { get; private set; }
    
    public string? Description { get;  private set; }
    
    //Ef Core
    private EventDetails() { }

    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }
}