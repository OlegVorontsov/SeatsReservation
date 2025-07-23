namespace SeatsReservation.Application.Shared.DTOs;

public record AvailableSeatDtoDapper
{
    public Guid Id { get; init; }

    public Guid VenueId { get; init; }

    public int SeatNumber { get; init; }

    public int RowNumber { get; init; }
    
    public bool IsAvailable { get; init; }
}