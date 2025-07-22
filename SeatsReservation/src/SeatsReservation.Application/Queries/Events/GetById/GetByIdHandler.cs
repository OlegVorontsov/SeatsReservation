using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Events;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Queries.Events.GetById;

public class GetByIdHandler(
    IValidator<GetByIdQuery> validator,
    IReadDbContext readDbContext) :
    IQueryHandlerWithResult<EventDto, GetByIdQuery>
{
    public async Task<Result<EventDto, ErrorList>> Handle(
        GetByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var eventResult = await readDbContext.EventsRead
            .Include(e => e.Details)
            .Where(e => e.Id == Id<Event>.Create(query.EventId))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (eventResult is null)
            return Error.NotFound("not.found", "Event not found").ToErrors();
        
        var seats = await readDbContext.SeatRead
            .Where(s => s.VenueId == eventResult.VenueId)
            .OrderBy(s => s.RowNumber)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
        
        return EventDto.FromDomainEntity(
            eventResult,
            seats.Select(SeatDto.FromDomainEntity).ToList());
    }
}