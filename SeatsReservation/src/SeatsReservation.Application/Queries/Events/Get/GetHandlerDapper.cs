using CSharpFunctionalExtensions;
using Dapper;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Queries.Events.Get;

public class GetHandlerDapper(
    IDbConnectionFactory connectionFactory) :
    IQueryHandlerWithResult<GetEventsDapperDto, GetQuery>
{
    public async Task<Result<GetEventsDapperDto, ErrorList>> Handle(
        GetQuery query, CancellationToken cancellationToken = default)
    {
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("e.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            conditions.Add("e.status = @status");
            parameters.Add("status", query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            conditions.Add("e.type = @event_type");
            parameters.Add("event_type", query.EventType);
        }

        if (query.DateFrom.HasValue)
        {
            conditions.Add("e.event_date >= @date_from");
            parameters.Add("date_from", query.DateFrom.Value.ToUniversalTime());
        }

        if (query.DateTo.HasValue)
        {
            conditions.Add("e.event_date <= @date_to");
            parameters.Add("date_to", query.DateTo.Value.ToUniversalTime());
        }

        if (query.VenueId.HasValue)
        {
            conditions.Add("e.venue_id = @venue_id");
            parameters.Add("venue_id", query.VenueId.Value);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            conditions.Add("""
                           ((SELECT COUNT(*) FROM seats s WHERE s.venue_id = e.venue_id) - 
                            COALESCE((SELECT COUNT(*)
                                      FROM reservation_seats rs
                                               JOIN reservations r ON rs.reservation_id = r.id
                                      WHERE rs.event_id = e.id
                                        AND r.status IN ('Confirmed', 'Pending')), 0)) >= @min_available_seats
                           """);
            parameters.Add("min_available_seats", query.MinAvailableSeats.Value);
        }

        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize);
        parameters.Add("page_size", query.Pagination.PageSize);

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
        
        var direction = query.SortDirection?.ToLower() == "asc" ? "ASC" : "DESC";

        var orderByField = query.SortBy?.ToLower() switch
        {
            "date" => "event_date",
            "name" => "name",
            "status" => "status",
            "type" => "type",
            "popularity" => "popularity_percentage",
            _ => "event_date"
        };

        var orderByClause = $"ORDER BY {orderByField} {direction}";

        long? totalCount = null;

        var events = await connection.QueryAsync<EventWithoutSeatsDtoDapper, long, EventWithoutSeatsDtoDapper>(
            $"""
              SELECT e.id,
                     e.name,
                     e.event_date,
                     ed.capacity,
                     ed.description,
                     e.venue_id,
                     e.event_type,
                     e.event_info,
                     e.started_at,
                     e.ended_at,
                     e.status,

                     (SELECT COUNT(*)
                      FROM seats_reservation.seats s
                      WHERE s.venue_id = e.venue_id)                        as total_seats,

                     (SELECT COUNT(*)
                      FROM seats_reservation.reservation_seats rs
                      JOIN seats_reservation.reservations r ON rs.reservation_id = r.id
                      WHERE rs.event_id = e.id
                      AND r.reservation_status IN ('Confirmed', 'Pending')) as reserved_seats,

                      COUNT(*) OVER ()                                      as total_count

                      FROM seats_reservation.events e
                      JOIN seats_reservation.event_details ed ON e.id = ed.event_id
                      {whereClause}
              {orderByClause}
              LIMIT @page_size OFFSET @offset;
              """,
            splitOn: "total_count",
            map: (@event, count) =>
            {
                totalCount ??= count;

                return @event;
            },
            param: parameters);

        return new GetEventsDapperDto(events.ToList(), totalCount ?? 0);
    }
}