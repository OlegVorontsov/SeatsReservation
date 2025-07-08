using System.Data;

namespace SeatsReservation.Infrastructure.Postgres.Interfaces;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}