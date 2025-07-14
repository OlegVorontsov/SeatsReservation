using Microsoft.AspNetCore.Mvc;
using SeatsReservation.Application.Commands.Reservations.CreateReservation;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Framework;
using SharedService.Framework.EndpointResults;

namespace SeatsReservation.Presentation.Controllers;

public class ReservationsController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<ReservationDto>> Create(
        [FromServices] CreateReservationHandler handler,
        [FromBody] CreateReservationCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
}