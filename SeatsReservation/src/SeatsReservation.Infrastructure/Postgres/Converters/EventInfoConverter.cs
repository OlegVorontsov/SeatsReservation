using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeatsReservation.Domain.Entities.Events.EventInfo;

namespace SeatsReservation.Infrastructure.Postgres.Converters;

public class EventInfoConverter : ValueConverter<IEventInfo, string>
{
    public EventInfoConverter() : base(i => InfoToString(i),
        s => StringToInfo(s))
    {
    }
    private static string InfoToString(IEventInfo info) => info switch
    {
        ConferenceInfo c => $"Conference:{c.Speaker}|{c.Topic}",
        ConcertInfo c => $"Concert:{c.Performer}",
        OnlineInfo o => $"Online:{o.Url}",
        _ => throw new NotSupportedException("Unknown event info")
    };

    private static IEventInfo StringToInfo(string info)
    {
        var split = info.Split(':', 2);
        var type = split[0];
        var data = split[1];

        return type switch
        {
            "Concert" => new ConcertInfo(data),
            "Conference" => new ConferenceInfo(data.Split('|')[0], data.Split('|')[1]),
            "Online" => new OnlineInfo(data),
            _ => throw new NotSupportedException($"Unknown type: {type}")
        };
    }
}