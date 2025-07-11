using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateSeats;

public class UpdateSeatsHandler(
    IValidator<UpdateSeatsCommand> validator,
    IVenuesRepository repository)
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
        
        var venueResult = await repository.GetById(venueId, cancellationToken);
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();
        
        List<Seat> seats = [];
        foreach (var seatDto in command.Seats)
        {
            var seatResult = Seat.Create(venueId,
                seatDto.SeatNumber, seatDto.RowNumber);
            if (seatResult.IsFailure)
                return seatResult.Error.ToErrors();
            
            seats.Add(seatResult.Value);
        }

        var updateResult = venueResult.Value.UpdateSeats(seats);
        if(updateResult.IsFailure)
            return updateResult.Error.ToErrors();
        
        await repository.DeleteSeatsByVenueId(venueId, cancellationToken);
        
        await repository.SaveAsync(cancellationToken);
        
        return venueId.Value;
    }
}