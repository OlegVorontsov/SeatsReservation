using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Events.EventInfo;

namespace SeatsReservation.Application.Shared.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    DateTimeOffset EventDate,
    int Capacity,
    string? Description,
    Guid VenueId,
    string EventType,
    string? EventInfo,
    DateTimeOffset StartedAt,
    DateTimeOffset EndedAt,
    string EventStatus,
    IReadOnlyList<AvailableSeatDto> Seats,
    int? TotalSeats,
    int? ReservedSeats,
    int? AvailableSeats
    )
{
    public static EventDto FromDomainEntity(
        Event entity,
        IReadOnlyList<AvailableSeatDto> seats,
        int? totalSeats,
        int? reservedSeats,
        int? availableSeats
        ) =>
        new(
            entity.Id.Value,
            entity.Name,
            entity.EventDate,
            entity.Details.Capacity,
            entity.Details.Description,
            entity.VenueId.Value,
            entity.EventType.ToString(),
            entity.EventInfo.ToString(),
            entity.StartedAt,
            entity.EndedAt,
            entity.Status.ToString(),
            seats,
            totalSeats,
            reservedSeats,
            availableSeats);
}