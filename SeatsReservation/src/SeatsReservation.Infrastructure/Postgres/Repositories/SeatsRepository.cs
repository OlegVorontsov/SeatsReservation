using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Commands.Seats;
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
}