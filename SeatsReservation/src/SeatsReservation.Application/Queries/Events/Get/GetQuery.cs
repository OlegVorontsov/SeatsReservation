using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Queries.Events.Get;

public record GetQuery(
    string? Search,
    string? EventType,
    DateTime? DateFrom,
    DateTime? DateTo,
    string? Status,
    Guid? VenueId,
    int? MinAvailableSeats,
    PaginationRequest Pagination,
    string? SortBy = "date",
    string? SortDirection = "asc"
    ) : IQuery;

    public record PaginationRequest(int Page = 1, int PageSize = 20);