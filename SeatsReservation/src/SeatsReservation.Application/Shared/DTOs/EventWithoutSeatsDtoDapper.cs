namespace SeatsReservation.Application.Shared.DTOs;

// для Dapper нужен Dto с полями
public record EventWithoutSeatsDtoDapper
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public DateTimeOffset EventDate { get; init; }

    public int Capacity { get; init; }

    public string Description { get; init; } = string.Empty;

    public Guid VenueId { get; init; }

    public string EventType { get; init; } = string.Empty;

    public string EventInfo { get; init; } = null!;

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset EndedAt { get; init; }

    public string Status { get; init; } = string.Empty;
    
    public int TotalSeats { get; init; }

    public int ReservedSeats { get; init; }
}