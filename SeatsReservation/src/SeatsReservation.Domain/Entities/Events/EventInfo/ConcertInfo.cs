namespace SeatsReservation.Domain.Entities.Events.EventInfo;

public record ConcertInfo(string Performer) : IEventInfo;