namespace SeatsReservation.Domain.Entities.Events.EventInfo;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo
{
    public override string ToString() => $"Conference: {Topic}-{Speaker}";
}