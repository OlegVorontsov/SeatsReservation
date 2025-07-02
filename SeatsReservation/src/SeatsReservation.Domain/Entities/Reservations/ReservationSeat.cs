using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Domain.Entities.Reservations;

public class ReservationSeat
{
    public Id<ReservationSeat> Id { get; }

    public Reservation Reservation { get; private set; } = null!;
    
    public Id<Seat> SeatId { get; private set; }
    
    public DateTimeOffset ReservationDate { get; }
    
    //Ef Core
    private ReservationSeat() { }

    public ReservationSeat(Id<ReservationSeat> id, Reservation reservation, Id<Seat> seatId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservationDate = DateTimeOffset.UtcNow;
    }
}