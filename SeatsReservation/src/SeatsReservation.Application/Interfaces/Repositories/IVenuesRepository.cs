using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Interfaces.Repositories;

public interface IVenuesRepository
{
    Task<Result<Venue, Error>> CreateAsync(
        Venue entity, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> UpdateNameByPrefix(
        string prefix, VenueName venueName, CancellationToken cancellationToken = default);

    Task<Result<Venue, Error>> GetById(
        Id<Venue> id, CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken = default);
}