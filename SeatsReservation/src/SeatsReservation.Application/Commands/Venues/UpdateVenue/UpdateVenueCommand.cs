using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenue;

public record UpdateVenueCommand(
    string Name,
    string Prefix,
    int SeatsLimit,
    Guid Id) : ICommand;