namespace SeatsReservation.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; }
    
    public ReservationStatus Status { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    private List<ReservationSeat> _reservedSeats = [];
    
    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservedSeats;

    public Reservation(Guid id, Guid eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;

        var reservedSeats = seatIds
            .Select(seatId => new ReservationSeat(Guid.NewGuid(), this, seatId))
            .ToList();
        
        _reservedSeats = reservedSeats;
    }

}