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
    public async Task<Result<Venue, Error>> GetById(
        Id<Venue> id, CancellationToken cancellationToken)
    {
        var venue = await context.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        
        return venue is null
            ? Error.NotFound("venue.not.found", "Venue not found")
            : venue;
    }
    
    public async Task SaveAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
    
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
    
    public async Task<UnitResult<Error>> UpdateNameByPrefix(
        string prefix, VenueName venueName, CancellationToken cancellationToken)
    {
        await context.Venues
            .Where(v => v.Name.Prefix.StartsWith(prefix))
            .ExecuteUpdateAsync(setter => // отправляет запрос сразу
                setter.SetProperty(v => v.Name.Name, venueName.Name), cancellationToken);

        return UnitResult.Success<Error>();
    }
    
    public async Task UpdateAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        context.Venues.Update(venue);

        await context.SaveChangesAsync(cancellationToken);
    }
}