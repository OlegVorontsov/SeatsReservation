using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SeatsReservation.Infrastructure.Postgres.Interfaces;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Repositories;

public class NpgSqlVenuesRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<NpgSqlVenuesRepository> logger) : IVenuesRepository
{

    public async Task<Result<Venue, Error>> GetById(
        Id<Venue> id, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
    
    public async Task SaveAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
    
    public async Task<Result<Venue, Error>> CreateAsync(Venue venue, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        
        using var transaction = connection.BeginTransaction();

        try
        {
            const string venueInsertSql = """
                                          INSERT INTO seats_reservation.venues (id, name, prefix, seats_limit)
                                          VALUES (@Id, @Name, @Prefix, @SeatsLimit)
                                          """;
            var venueInsertParams = new
            {
                Id = venue.Id.Value,
                Name = venue.Name.Name,
                Prefix = venue.Name.Prefix,
                SeatsLimit = venue.SeatsLimit
            };
        
            await connection.ExecuteAsync(venueInsertSql, venueInsertParams);

            if (!venue.Seats.Any()) return venue;
        
            const string seatsInsertSql = """
                                          INSERT INTO seats_reservation.seats (id, seat_number, row_number, venue_id)
                                          VALUES (@Id, @SeatNumber, @RowNumber, @VenueId)
                                          """;
            var seatsInsertParams = venue.Seats.Select(s => new
            {
                Id = s.Id.Value,
                SeatNumber = s.SeatNumber,
                RowNumber = s.RowNumber,
                VenueId = venue.Id.Value
            });
        
            await connection.ExecuteAsync(seatsInsertSql, seatsInsertParams);
            
            transaction.Commit();
        
            return venue;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.LogError(ex, "Fail to insert venue");
            return Error.Failure("venue.insert", ex.Message);
        }
    }
    
    public async Task<Result<Guid, Error>> UpdateName(
        Id<Venue> id, VenueName venueName, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        
        using var transaction = connection.BeginTransaction();

        try
        {
            const string updateNameSql = """
                                          UPDATE seats_reservation.venues
                                          SET name = @Name
                                          WHERE id = @Id
                                          """;
            var updateNameParams = new
            {
                Id = id.Value,
                Name = venueName.Name,
            };
        
            await connection.ExecuteAsync(updateNameSql, updateNameParams);
            
            transaction.Commit();
        
            return id.Value;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.LogError(ex, "Fail to update venue name");
            return Error.Failure("venue.update", ex.Message);
        }
    }

    public async Task<UnitResult<Error>> UpdateNameByPrefix(
        string prefix, VenueName venueName, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        
        using var transaction = connection.BeginTransaction();

        try
        {
            const string updateNameSql = """
                                         UPDATE seats_reservation.venues
                                         SET name = @Name
                                         WHERE prefix LIKE @Prefix
                                         """;
            var updateNameParams = new
            {
                Prefix = $"{prefix}%",
                Name = venueName.Name,
            };
        
            await connection.ExecuteAsync(updateNameSql, updateNameParams);
            
            transaction.Commit();
        
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.LogError(ex, "Fail to update venue name");
            return Error.Failure("venue.update", ex.Message);
        }
    }
}