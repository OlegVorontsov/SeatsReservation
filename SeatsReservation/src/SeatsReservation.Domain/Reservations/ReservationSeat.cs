namespace SeatsReservation.Domain.Reservations;

public class ReservationSeat
{
    public Guid Id { get; }
    
    public Reservation Reservation { get; private set; }
    
    public Guid SeatId { get; private set; }
    
    public DateTimeOffset ReservationDate { get; }

    public ReservationSeat(Guid id, Reservation reservation, Guid seatId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservationDate = DateTimeOffset.UtcNow;
    }
}