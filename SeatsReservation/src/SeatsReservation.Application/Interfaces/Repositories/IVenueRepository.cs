using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<Result<Venue>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default);
}