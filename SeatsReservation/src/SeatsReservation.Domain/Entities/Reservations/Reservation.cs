using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Domain.Entities.Reservations;

public class Reservation
{
    public Id<Reservation> Id { get; } = null!;
    
    public ReservationStatus Status { get; private set; }
    
    public Id<Event> EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    private List<ReservationSeat> _reservedSeats = [];
    
    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservedSeats;
    
    //Ef Core
    private Reservation() { }

    public Reservation(Id<Reservation> id, Id<Event> eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;

        var reservedSeats = seatIds
            .Select(seatId => new ReservationSeat(
                Id<ReservationSeat>.Create(Guid.NewGuid()), this, Id<Seat>.Create(seatId)))
            .ToList();
        
        _reservedSeats = reservedSeats;
    }
}