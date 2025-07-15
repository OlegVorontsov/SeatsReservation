using Microsoft.AspNetCore.Mvc;
using SeatsReservation.Application.Commands.Events.CreateEvent;
using SeatsReservation.Application.Commands.Reservations.CreateReservation;
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
}