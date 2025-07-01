using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Venues;

namespace SeatsReservation.Application.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<Result<Venue>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default);
}