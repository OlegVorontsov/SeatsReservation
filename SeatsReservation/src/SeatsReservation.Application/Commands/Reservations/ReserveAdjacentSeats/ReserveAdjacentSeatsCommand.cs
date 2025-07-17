using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Reservations.ReserveAdjacentSeats;

public record ReserveAdjacentSeatsCommand(
    Guid EventId,
    Guid UserId,
    Guid VenueId,
    int RequiredSeatsCount,
    int? PreferredRowNumber) : ICommand;