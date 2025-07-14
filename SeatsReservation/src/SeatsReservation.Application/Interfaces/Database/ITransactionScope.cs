using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Interfaces.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();
    UnitResult<Error> Rollback();
}