using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

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

    private Reservation(Id<Reservation> id, Id<Event> eventId, Guid userId, IEnumerable<Guid> seatIds)
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
    
    public static Result<Reservation, Error> Create(
        Guid eventId, Guid userId, IEnumerable<Guid> seatsIds)
    {
        var seatsIdsList = seatsIds.ToList();
        
        if (seatsIdsList.Count == 0)
            return Error.Validation("reservation.seats", "At least one seat must be selected");
        
        return new Reservation(
            Id<Reservation>.Create(Guid.NewGuid()),
            Id<Event>.Create(eventId),
            userId,
            seatsIdsList);
    }
}