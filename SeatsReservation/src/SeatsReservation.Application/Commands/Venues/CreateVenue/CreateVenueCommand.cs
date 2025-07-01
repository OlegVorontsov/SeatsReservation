using SeatsReservation.Application.Shared.DTOs;
using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Venues.CreateVenue;

public record CreateVenueCommand(
    string Name,
    int SeatsLimit,
    IEnumerable<SeatDto> Seats) : ICommand;