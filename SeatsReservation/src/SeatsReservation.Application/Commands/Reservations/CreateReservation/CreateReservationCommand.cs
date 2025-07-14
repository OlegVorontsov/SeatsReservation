using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Commands.Reservations.CreateReservation;

public record CreateReservationCommand(
    Guid EventId,
    Guid UserId,
    IEnumerable<Guid> SeatsIds) : ICommand;