using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Core.Abstractions;
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

        var events = await eventsQuery
            .Include(e => e.Details)
            .Select(e => EventWithoutSeatsDto.FromDomainEntity(e))
            .ToListAsync(cancellationToken);
        
        return new GetEventsDto(events);
    }
}