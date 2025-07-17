using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Domain.Entities.Reservations;

// таблица связи "многие - ко многим"
public class ReservationSeat
{
    public Id<ReservationSeat> Id { get; }

    public Reservation Reservation { get; private set; } = null!;
    
    public Id<Seat> SeatId { get; private set; }
    
    public Id<Event> EventId { get; private set; }

    public DateTimeOffset ReservationDate { get; }
    
    //Ef Core
    private ReservationSeat() { }

    public ReservationSeat(
        Id<ReservationSeat> id,
        Reservation reservation,
        Id<Seat> seatId,
        Id<Event> eventId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        EventId = eventId;
        ReservationDate = DateTimeOffset.UtcNow;
    }
}