using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Queries.Events.Get;

public record GetQuery(
    string? Search,
    string? EventType,
    DateTime? DateFrom,
    DateTime? DateTo,
    string? Status,
    Guid? VenueId
    ) : IQuery;