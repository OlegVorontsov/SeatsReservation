using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SeatsReservation.Application.Interfaces.Database;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Infrastructure.Postgres.Write;

public class TransactionManager(
    ApplicationWriteDbContext context,
    ILogger<TransactionManager> logger,
    ILoggerFactory loggerFactory) : ITransactionManager
{
    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        IsolationLevel? isolationLevel = null)
    {
        try
        {
            var transaction = await context.Database.BeginTransactionAsync(
                isolationLevel ?? IsolationLevel.ReadCommitted,
                cancellationToken);
            
            var transactionScope = new TransactionScope(
                transaction.GetDbTransaction(),
                loggerFactory.CreateLogger<TransactionScope>());
            return transactionScope;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Failed to begin transaction");
            return Error.Failure("database", "Failed to begin transaction");
        }
    }
    
    public async Task<UnitResult<Error>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Failed to save changes");
            return Error.Failure("database", "Failed to save changes");
        }
    }
}