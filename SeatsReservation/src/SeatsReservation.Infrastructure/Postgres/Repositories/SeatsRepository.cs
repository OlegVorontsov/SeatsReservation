using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Commands.Seats;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Write;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Repositories;

public class SeatsRepository(
    ApplicationWriteDbContext context,
    ILogger<SeatsRepository> logger)
    : ISeatsRepository
{
    public async Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<Id<Seat>> seatIds, CancellationToken cancellationToken = default) =>
    await context.Seats
        .Where(s => seatIds.Contains(s.Id))
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Seat>> GetAvailableSeats(
        Id<Venue> venueId, Id<Event> eventId, int? rowNumber, CancellationToken cancellationToken = default)
    {
        return await context.Seats
            .Where(s => s.VenueId == venueId)
            .Where(s => rowNumber.HasValue && s.RowNumber == rowNumber)
            .Where(s => !context.ReservationSeats.Any(
                            rs => rs.SeatId == s.Id &&
                                  rs.EventId == eventId &&
                                  (rs.Reservation.Status == ReservationStatus.Confirmed ||
                                   rs.Reservation.Status == ReservationStatus.Pending)))
            .ToListAsync(cancellationToken);
    }
}