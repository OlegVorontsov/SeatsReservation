using Microsoft.AspNetCore.Mvc;
using SeatsReservation.Application.Commands.Venues.CreateVenue;
using SeatsReservation.Application.Commands.Venues.UpdateVenueName;
using SeatsReservation.Application.Shared.DTOs;
using SharedService.Framework;
using SharedService.Framework.EndpointResults;

namespace SeatsReservation.Presentation.Controllers;

public class VenuesController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<VenueDto>> Create(
        [FromServices] CreateVenueHandler handler,
        [FromBody] CreateVenueCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
    
    [HttpPut]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] UpdateVenueNameHandler handler,
        [FromBody] UpdateVenueNameCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
}