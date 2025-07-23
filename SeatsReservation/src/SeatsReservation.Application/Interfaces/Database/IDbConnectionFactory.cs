using System.Data;

namespace SeatsReservation.Application.Interfaces.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}