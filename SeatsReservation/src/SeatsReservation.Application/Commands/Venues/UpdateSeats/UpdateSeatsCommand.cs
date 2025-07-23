using SeatsReservation.Application.Shared.DTOs;
using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Venues.UpdateSeats;

public record UpdateSeatsCommand(
    Guid Id,
    IEnumerable<AvailableSeatDto> Seats) : ICommand;