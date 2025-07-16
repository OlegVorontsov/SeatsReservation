using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

public class ReservationsRepository(
    ApplicationWriteDbContext context,
    ILogger<ReservationsRepository> logger)
    : IReservationsRepository
{
    public async Task<Result<Reservation, Error>> GetById(
        Id<Reservation> id, CancellationToken cancellationToken)
    {
        var reservation = await context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        
        return reservation is null
            ? Error.NotFound("reservation.not.found", "Reservation not found")
            : reservation;
    }
    
    public async Task<Result<Reservation, Error>> CreateAsync(
        Reservation entity, CancellationToken cancellationToken = default)
    {
        try
        {
            context.Reservations.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to insert reservation");
            return Error.Failure("reservation.insert", ex.Message);
        }
    }

    public async Task<bool> IsAnySeatsReserved(
        Id<Event> eventId, IEnumerable<Id<Seat>> seatIds,
        CancellationToken cancellationToken = default) =>
        await context.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(cancellationToken);

    public async Task<int> GetReservedSeatsCount(
        Id<Event> eventId, CancellationToken cancellationToken = default)
    {
        // создание пессимистичной блокировки
        /*await context.Database.ExecuteSqlAsync(
            $"SELECT capacity FROM seats_reservation.events WHERE id = {eventId} FOR UPDATE",
            cancellationToken);*/
        
        return await context.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending)
            .SelectMany(r => r.ReservedSeats)
            .CountAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Reservation entity, CancellationToken cancellationToken = default)
    {
        context.Reservations.Update(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}