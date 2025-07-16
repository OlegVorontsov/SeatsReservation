using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Reservations;

public interface IReservationsRepository
{
    Task<Result<Reservation, Error>> CreateAsync(
        Reservation entity, CancellationToken cancellationToken = default);

    Task<Result<Reservation, Error>> GetById(
        Id<Reservation> id, CancellationToken cancellationToken = default);

    Task<bool> IsAnySeatsReserved(
        Id<Event> eventId, IEnumerable<Id<Seat>> seatIds,
        CancellationToken cancellationToken = default);

    Task<int> GetReservedSeatsCount(
        Id<Event> eventId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Reservation venue, CancellationToken cancellationToken = default);
}