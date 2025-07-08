using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SeatsReservation.Infrastructure.Postgres.Write;
using SharedService.SharedKernel.BaseClasses;
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

    public async Task<Result<Guid, Error>> UpdateName(
        Id<Venue> id, VenueName venueName, CancellationToken cancellationToken)
    {
        try
        {
            var venue = await context.Venues
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
            if (venue == null)
                return Error.NotFound("venue.update", "Venue not found");
        
            venue.UpdateName(venueName);
            await context.SaveChangesAsync(cancellationToken);

            return venue.Id.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to update venue name");
            return Error.Failure("venue.update", ex.Message);
        }

    }
}