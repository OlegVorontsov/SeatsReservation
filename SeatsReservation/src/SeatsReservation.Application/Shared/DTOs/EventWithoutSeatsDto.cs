using SeatsReservation.Domain.Entities.Events;

namespace SeatsReservation.Application.Shared.DTOs;

public record EventWithoutSeatsDto(
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
    int? TotalSeats,
    int? ReservedSeats,
    int? AvailableSeats
    )
{
    public static EventWithoutSeatsDto FromDomainEntity(
        Event entity,
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
            totalSeats,
            reservedSeats,
            availableSeats
            );
}