using SeatsReservation.Domain.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record SeatDto(
    int SeatNumber,
    int RowNumber)
{
    public static SeatDto FromDomainEntity(Seat entity)
        => new(
            entity.SeatNumber,
            entity.RowNumber);
}