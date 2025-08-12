namespace SeatsReservation.Application.Shared.DTOs;

public record GetEventsDto(List<EventWithoutSeatsDto> Events, long TotalCount);