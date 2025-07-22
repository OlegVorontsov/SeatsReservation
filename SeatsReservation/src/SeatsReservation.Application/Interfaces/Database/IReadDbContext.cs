using SeatsReservation.Domain.Entities.Events;

namespace SeatsReservation.Application.Interfaces.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }
}