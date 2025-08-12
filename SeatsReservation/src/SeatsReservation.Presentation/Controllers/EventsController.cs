using Microsoft.AspNetCore.Mvc;
using SeatsReservation.Application.Commands.Events.CreateEvent;
using SeatsReservation.Application.Queries.Events.Get;
using SeatsReservation.Application.Queries.Events.GetById;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Framework;
using SharedService.Framework.EndpointResults;

namespace SeatsReservation.Presentation.Controllers;

public class EventsController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<EventDto>> Create(
        [FromServices] CreateEventHandler handler,
        [FromBody] CreateEventCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);

    [HttpGet("{eventId:guid}")]
    public async Task<EndpointResult<EventDto>> GetById(
        [FromServices] GetByIdHandler handler,
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new GetByIdQuery(eventId), cancellationToken);
    
    [HttpGet("{eventId:guid}/dapper")]
    public async Task<EndpointResult<EventDtoDapper?>> GetByIdDapper(
        [FromServices] GetByIdHandlerDapper handler,
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new GetByIdQuery(eventId), cancellationToken);
    
    [HttpGet]
    public async Task<EndpointResult<GetEventsDto>> Get(
        [FromQuery] GetQuery query,
        [FromServices] GetHandler handler,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(query, cancellationToken);
    
    [HttpGet("/dapper")]
    public async Task<EndpointResult<GetEventsDapperDto>> GetDapper(
        [FromQuery] GetQuery query,
        [FromServices] GetHandlerDapper handler,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(query, cancellationToken);
}