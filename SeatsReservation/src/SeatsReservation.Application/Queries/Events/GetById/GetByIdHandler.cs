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

        var eventId = Id<Event>.Create(query.EventId);
        
        var eventResult = await readDbContext.EventsRead
            .Include(e => e.Details)
            .Where(e => e.Id == eventId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (eventResult is null)
            return Error.NotFound("not.found", "Event not found").ToErrors();
        
        var seats = await readDbContext.SeatRead
            .Where(s => s.VenueId == eventResult.VenueId)
            .OrderBy(s => s.RowNumber)
            .ThenBy(s => s.SeatNumber)
            .Select(s => AvailableSeatDto.FromDomainEntity(s,
                !readDbContext.ReservationSeatRead
                    .Any(rs => rs.SeatId == s.Id && rs.EventId == eventId)))
            .ToListAsync(cancellationToken);
        
        return EventDto.FromDomainEntity(
            eventResult, seats);
    }
}