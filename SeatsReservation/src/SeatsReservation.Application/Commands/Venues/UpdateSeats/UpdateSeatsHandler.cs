using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateSeats;

public class UpdateSeatsHandler(
    IValidator<UpdateSeatsCommand> validator,
    IVenuesRepository repository,
    ITransactionManager transactionManager)
    : ICommandHandler<Guid, UpdateSeatsCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateSeatsCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var venueId = Id<Venue>.Create(command.Id);
        
        var transactionScopeResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();
        
        using var transactionScope = transactionScopeResult.Value;
        
        var venueResult = await repository.GetById(venueId, cancellationToken);
        if (venueResult.IsFailure)
        {
            transactionScope.Rollback();
            return venueResult.Error.ToErrors();
        }
        
        List<Seat> seats = [];
        foreach (var seatDto in command.Seats)
        {
            var seatResult = Seat.Create(venueId,
                seatDto.SeatNumber, seatDto.RowNumber);
            if (seatResult.IsFailure)
            {
                transactionScope.Rollback();
                return seatResult.Error.ToErrors();
            }
            seats.Add(seatResult.Value);
        }

        var updateResult = venueResult.Value.UpdateSeats(seats);
        if (updateResult.IsFailure)
        {
            transactionScope.Rollback();
            return updateResult.Error.ToErrors();
        }
        
        await repository.DeleteSeatsByVenueId(venueId, cancellationToken);
        
        await transactionManager.SaveChangesAsync(cancellationToken);
        
        var commitedResult = transactionScope.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();
        
        return venueId.Value;
    }
}