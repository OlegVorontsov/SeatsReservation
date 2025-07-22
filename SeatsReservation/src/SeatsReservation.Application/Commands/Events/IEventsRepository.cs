using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Events;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Events;

public interface IEventsRepository
{
    Task<Result<Event, Error>> GetByIdWithLock(
        Id<Event> id, CancellationToken cancellationToken = default);

    Task<Result<Event, Error>> CreateAsync(
        Event entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Event entity, CancellationToken cancellationToken = default);
}