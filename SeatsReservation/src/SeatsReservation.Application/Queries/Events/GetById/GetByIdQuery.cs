using SharedService.Core.Abstractions;

namespace SeatsReservation.Application.Queries.Events.GetById;

public record GetByIdQuery(Guid EventId) : IQuery;