using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record AvailableSeatDto(
    Guid Id,
    Guid VenueId,
    int SeatNumber,
    int RowNumber,
    bool IsAvailable)
{
    public static AvailableSeatDto FromDomainEntity(Seat entity, bool isAvailable)
        => new(
            entity.Id.Value,
            entity.VenueId.Value,
            entity.SeatNumber,
            entity.RowNumber,
            isAvailable);
}