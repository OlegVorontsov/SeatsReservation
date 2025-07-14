using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Interfaces.Database;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Write;

public class TransactionScope(
    IDbTransaction transaction,
    ILogger<TransactionScope> logger) : ITransactionScope
{
    public UnitResult<Error> Commit()
    {
        try
        {
            transaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Failed to commit transaction");
            return UnitResult.Failure(Error.Failure("database", "Failed to commit transaction"));
        }
    }
    
    public UnitResult<Error> Rollback()
    {
        try
        {
            transaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Failed to rollback transaction");
            return UnitResult.Failure(Error.Failure("database", "Failed to rollback transaction"));
        }
    }

    public void Dispose() => transaction.Dispose();
}