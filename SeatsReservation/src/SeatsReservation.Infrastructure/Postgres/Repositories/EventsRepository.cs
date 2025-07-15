using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Commands.Events;
using SeatsReservation.Application.Commands.Reservations;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SeatsReservation.Infrastructure.Postgres.Write;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Repositories;

public class EventsRepository(
    ApplicationWriteDbContext context,
    ILogger<EventsRepository> logger)
    : IEventsRepository
{
    public async Task<Result<Event, Error>> GetById(
        Id<Event> id, CancellationToken cancellationToken = default)
    {
        var @event = await context.Events
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        
        return @event is null
            ? Error.NotFound("event.not.found", "Event not found")
            : @event;
    }
    
    public async Task<Result<Event, Error>> CreateAsync(
        Event entity, CancellationToken cancellationToken = default)
    {
        try
        {
            context.Events.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to insert event");
            return Error.Failure("event.insert", ex.Message);
        }
    }
    
    public async Task UpdateAsync(Event entity, CancellationToken cancellationToken = default)
    {
        context.Events.Update(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}