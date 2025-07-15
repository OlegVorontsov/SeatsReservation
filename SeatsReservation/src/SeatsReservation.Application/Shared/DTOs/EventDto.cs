using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Events.EventInfo;

namespace SeatsReservation.Application.Shared.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    DateTimeOffset EventDate,
    int Capacity,
    string? EventDescription,
    Guid VenueId,
    string EventType,
    string? Performer,
    string? Speaker,
    string? Topic,
    string? Url,
    DateTimeOffset StartedAt,
    DateTimeOffset EndedAt,
    string EventStatus)
{
    public static EventDto FromDomainEntity(Event entity)
    {
        string? performer = null;
        string? speaker = null;
        string? topic = null;
        string? url = null;
        
        switch (entity.EventInfo)
        {
            case ConcertInfo concert:
                performer = concert.Performer;
                break;
            case ConferenceInfo conference:
                speaker = conference.Speaker;
                topic = conference.Topic;
                break;
            case OnlineInfo online:
                url = online.Url;
                break;
        }
        return new EventDto(
            entity.Id.Value,
            entity.Name,
            entity.EventDate,
            entity.Details.Capacity,
            entity.Details.Description,
            entity.VenueId.Value,
            entity.EventType.ToString(),
            performer,
            speaker,
            topic,
            url,
            entity.StartedAt,
            entity.EndedAt,
            entity.Status.ToString()
        );
    }
}