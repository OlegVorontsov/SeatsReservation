using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record SeatDto(
    Guid Id,
    Guid VenueId,
    int SeatNumber,
    int RowNumber)
{
    public static SeatDto FromDomainEntity(Seat entity)
        => new(
            entity.Id.Value,
            entity.VenueId.Value,
            entity.SeatNumber,
            entity.RowNumber);
}