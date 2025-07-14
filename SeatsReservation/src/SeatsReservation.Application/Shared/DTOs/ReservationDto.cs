using SeatsReservation.Domain.Entities.Reservations;

namespace SeatsReservation.Application.Shared.DTOs;

public record ReservationDto(
    Guid Id,
    string Status,
    Guid EventId,
    Guid UserId,
    DateTimeOffset CreatedAt,
    IEnumerable<ReservationSeatDto> ReservationSeats)
{
    public static ReservationDto FromDomainEntity(Reservation entity)
        => new(
            entity.Id.Value,
            entity.Status.ToString(),
            entity.EventId.Value,
            entity.UserId,
            entity.CreatedAt,
            entity.ReservedSeats.Select(rs =>
                new ReservationSeatDto(rs.SeatId.Value, rs.ReservationDate)));
}