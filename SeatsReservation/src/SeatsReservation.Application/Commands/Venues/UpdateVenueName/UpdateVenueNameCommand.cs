using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueName;

public record UpdateVenueNameCommand(
    Guid Id,
    string Name) : ICommand;