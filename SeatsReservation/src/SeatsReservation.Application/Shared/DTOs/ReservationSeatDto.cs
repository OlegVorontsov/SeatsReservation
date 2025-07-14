using SeatsReservation.Domain.Entities.Reservations;

namespace SeatsReservation.Application.Shared.DTOs;

public record ReservationSeatDto(
    Guid SeatId,
    DateTimeOffset ReservationDate)
{
    public static ReservationSeatDto FromDomainEntity(ReservationSeat entity)
        => new(
            entity.SeatId.Value,
            entity.ReservationDate);
}