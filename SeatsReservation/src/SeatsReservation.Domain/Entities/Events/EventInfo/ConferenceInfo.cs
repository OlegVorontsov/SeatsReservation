namespace SeatsReservation.Domain.Entities.Events.EventInfo;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo;