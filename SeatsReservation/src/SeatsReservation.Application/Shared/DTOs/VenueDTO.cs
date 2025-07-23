using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record VenueDto(
    Guid Id,
    string VenueName,
    int SeatsLimit,
    IEnumerable<AvailableSeatDto> Seats)
{
    public static VenueDto FromDomainEntity(Venue entity)
        => new(
            entity.Id.Value,
            entity.Name.ToString(),
            entity.SeatsLimit,
            entity.Seats
                .Select(s => new AvailableSeatDto(
                    s.Id.Value,
                    s.VenueId.Value,
                    s.SeatNumber,
                    s.RowNumber,
                    true)));
}