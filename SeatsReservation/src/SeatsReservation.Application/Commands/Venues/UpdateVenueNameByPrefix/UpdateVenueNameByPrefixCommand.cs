using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueNameByPrefix;

public record UpdateVenueNameByPrefixCommand(
    string Prefix,
    string Name) : ICommand;