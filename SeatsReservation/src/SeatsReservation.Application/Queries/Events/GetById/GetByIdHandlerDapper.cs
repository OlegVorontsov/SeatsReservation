using CSharpFunctionalExtensions;
using Dapper;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Queries.Events.GetById;

public class GetByIdHandlerDapper(
    IValidator<GetByIdQuery> validator,
    IDbConnectionFactory connectionFactory) :
    IQueryHandlerWithResult<EventDtoDapper?, GetByIdQuery>
{
    public async Task<Result<EventDtoDapper?, ErrorList>> Handle(
        GetByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        EventDtoDapper? eventDto = null;

        await connection.QueryAsync<EventDtoDapper, AvailableSeatDtoDapper, EventDtoDapper>(
            """
            SELECT
                   e.id,
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
                   COUNT(*) OVER () as total_seats,
                   COUNT(rs.seat_id) OVER () as reserved_seats,
                   COUNT(*) OVER () - COUNT(rs.seat_id) OVER () as available_seats,
                   s.id,
                   s.venue_id,
                   s.seat_number,
                   s.row_number,
                   rs is null as is_available
                   FROM seats_reservation.events e
            JOIN seats_reservation.event_details ed ON ed.event_id = e.id
            JOIN seats_reservation.seats s ON s.venue_id = e.venue_id
            LEFT JOIN seats_reservation.reservation_seats rs ON rs.seat_id = s.id AND rs.event_id = e.id
            WHERE e.id = @eventId
            ORDER BY s.row_number, s.seat_number
            """,
            param: new
            {
                eventId = query.EventId
            },
            splitOn: "id",
            map: (e, s) =>
            {
                eventDto ??= e;

                eventDto.Seats.Add(s);

                return eventDto;
            });

        return eventDto;
    }
}