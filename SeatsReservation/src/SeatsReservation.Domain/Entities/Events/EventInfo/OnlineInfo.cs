namespace SeatsReservation.Domain.Entities.Events.EventInfo;

public record OnlineInfo(string Url) : IEventInfo
{
    public override string ToString() => $"Online: {Url}";
}