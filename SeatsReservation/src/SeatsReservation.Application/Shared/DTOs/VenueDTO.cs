using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record VenueDto(
    Guid Id,
    string VenueName,
    int SeatsLimit,
    IEnumerable<SeatDto> Seats)
{
    public static VenueDto FromDomainEntity(Venue entity)
        => new(
            entity.Id.Value,
            entity.VenueName.ToString(),
            entity.SeatsLimit,
            entity.Seats.Select(s => new SeatDto(s.SeatNumber, s.RowNumber)));
}