using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Interfaces.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}