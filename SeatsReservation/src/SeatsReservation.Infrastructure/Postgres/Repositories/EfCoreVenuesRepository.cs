using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Write;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Repositories;

public class EfCoreVenuesRepository(
    ApplicationWriteDbContext context,
    ILogger<EfCoreVenuesRepository> logger)
    : IVenuesRepository
{
    public async Task<Result<Venue, Error>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default)
    {
        try
        {
            context.Venues.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to insert venue");
            return Error.Failure("venue.insert", ex.Message);
        }
    }
}