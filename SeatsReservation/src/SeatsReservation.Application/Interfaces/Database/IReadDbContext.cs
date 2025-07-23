using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Interfaces.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }
    IQueryable<Venue> VenueRead { get; }
    IQueryable<Seat> SeatRead { get; }
    IQueryable<Reservation> ReservationRead { get; }
    IQueryable<ReservationSeat> ReservationSeatRead { get; }
}