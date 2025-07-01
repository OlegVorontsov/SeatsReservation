using SeatsReservation.Domain.Venues;

namespace SeatsReservation.Application.Shared.DTOs;

public record VenueDto(
    Guid Id,
    string Name,
    int SeatsLimit,
    IEnumerable<SeatDto> Seats)
{
    public static VenueDto FromDomainEntity(Venue entity)
        => new(
            entity.Id,
            entity.Name,
            entity.SeatsLimit,
            entity.Seats.Select(s => new SeatDto(s.SeatNumber, s.RowNumber)));
}