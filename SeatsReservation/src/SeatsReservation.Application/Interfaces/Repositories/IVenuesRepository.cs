using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Interfaces.Repositories;

public interface IVenuesRepository
{
    Task<Result<Venue, Error>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default);
}