namespace SeatsReservation.Application.Shared.DTOs;

public record SeatDtoDapper
{
    public Guid Id { get; init; }

    public Guid VenueId { get; init; }

    public int SeatNumber { get; init; }

    public int RowNumber { get; init; }
}