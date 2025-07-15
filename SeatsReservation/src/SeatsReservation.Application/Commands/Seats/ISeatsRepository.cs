using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Seats;

public interface ISeatsRepository
{
    Task <IReadOnlyList<Seat>> GetByIds(
        IEnumerable<Id<Seat>> seatIds, CancellationToken cancellationToken = default);
}