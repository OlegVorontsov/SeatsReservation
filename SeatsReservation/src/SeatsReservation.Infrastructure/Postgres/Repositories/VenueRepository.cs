using CSharpFunctionalExtensions;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Write;

namespace SeatsReservation.Infrastructure.Postgres.Repositories;

public class VenueRepository(ApplicationWriteDbContext context)
    : IVenueRepository
{
    public async Task<Result<Venue>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default)
    {
        context.Venues.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}