using Microsoft.AspNetCore.Mvc;
using SeatsReservation.Application.Commands.Venues.CreateVenue;
using SeatsReservation.Application.Commands.Venues.UpdateSeats;
using SeatsReservation.Application.Commands.Venues.UpdateVenue;
using SeatsReservation.Application.Commands.Venues.UpdateVenueName;
using SeatsReservation.Application.Commands.Venues.UpdateVenueNameByPrefix;
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
    
    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdateName(
        [FromServices] UpdateVenueNameHandler handler,
        [FromBody] UpdateVenueNameCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
    
    [HttpPatch("name-by-prefix")]
    public async Task<EndpointResult<string>> UpdateNameByPrefix(
        [FromServices] UpdateVenueNameByPrefixHandler handler,
        [FromBody] UpdateVenueNameByPrefixCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
    
    [HttpPatch("seats")]
    public async Task<EndpointResult<Guid>> UpdateSeats(
        [FromServices] UpdateSeatsHandler handler,
        [FromBody] UpdateSeatsCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
    
    [HttpPut]
    public async Task<EndpointResult<VenueDto>> Update(
        [FromServices] UpdateVenueHandler handler,
        [FromBody] UpdateVenueCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
}