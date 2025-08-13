using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Queries.Events.Get;

public class GetHandler(
    IReadDbContext readDbContext) :
    IQueryHandlerWithResult<GetEventsDto, GetQuery>
{
    public async Task<Result<GetEventsDto, ErrorList>> Handle(
        GetQuery query, CancellationToken cancellationToken = default)
    {
        var eventsQuery = readDbContext.EventsRead;
        
        if(!string.IsNullOrWhiteSpace(query.Search))
            eventsQuery = eventsQuery
                .Where(e => EF.Functions.Like(e.Name.ToLower(), $"%{query.Search.ToLower()}%"));
        
        if(!string.IsNullOrWhiteSpace(query.EventType))
            eventsQuery = eventsQuery
                .Where(e => e.EventType.ToString().ToLower() == query.EventType.ToLower());
        
        if(query.DateFrom.HasValue)
            eventsQuery = eventsQuery.Where(e => e.EventDate >= query.DateFrom.Value.ToUniversalTime());
        
        if(query.DateTo.HasValue)
            eventsQuery = eventsQuery.Where(e => e.EventDate <= query.DateTo.Value.ToUniversalTime());
        
        if(query.VenueId.HasValue)
            eventsQuery = eventsQuery.Where(e => e.VenueId == Id<Venue>.Create(query.VenueId.Value));
        
        if(!string.IsNullOrWhiteSpace(query.Status))
            eventsQuery = eventsQuery
                .Where(e => e.Status.ToString().ToLower() == query.Status.ToLower());
        
        if (query.MinAvailableSeats.HasValue)
        {
            eventsQuery = eventsQuery.Where(e =>
                readDbContext.SeatRead.Count(s => s.VenueId == e.VenueId) -
                readDbContext.ReservationSeatRead.Count(rs =>
                    rs.EventId == e.Id &&
                    (rs.Reservation.Status == ReservationStatus.Confirmed ||
                     rs.Reservation.Status == ReservationStatus.Pending))
                >= query.MinAvailableSeats.Value);
        }

        Expression<Func<Event, object>> keySelector = query.SortBy?.ToLower() switch
        {
            "date" =>       e => e.EventDate,
            "name" =>       e => e.Name,
            "status" =>     e => e.Status,
            "type" =>       e => e.EventType,
            "popularity" => e => (double)readDbContext.ReservationSeatRead.Count(rs => rs.EventId == e.Id &&
                (rs.Reservation.Status == ReservationStatus.Confirmed ||
                 rs.Reservation.Status == ReservationStatus.Pending)) /
                 readDbContext.SeatRead.Count(s => s.VenueId == e.VenueId) * 100.0,
            _ =>            e => e.EventDate
        };

        eventsQuery = query.SortDirection == "asc" ?
            eventsQuery.OrderBy(keySelector) :
            eventsQuery.OrderByDescending(keySelector);
        
        var totalCount = await eventsQuery.LongCountAsync(cancellationToken);
        
        eventsQuery = eventsQuery
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Take(query.Pagination.PageSize);

        var events = await eventsQuery
            .Include(e => e.Details)
            .Select(e => EventWithoutSeatsDto.FromDomainEntity(
                e,
                readDbContext.SeatRead.Count(s => s.VenueId == e.VenueId),
                readDbContext.ReservationSeatRead.Count(rs => rs.EventId == e.Id),
                readDbContext.SeatRead.Count(s => s.VenueId == e.VenueId) -
                readDbContext.ReservationSeatRead.Count(rs => rs.EventId == e.Id &&
                                                              (rs.Reservation.Status == ReservationStatus.Confirmed ||
                                                               rs.Reservation.Status == ReservationStatus.Pending))))
            .ToListAsync(cancellationToken);
        
        return new GetEventsDto(events, totalCount);
    }
}