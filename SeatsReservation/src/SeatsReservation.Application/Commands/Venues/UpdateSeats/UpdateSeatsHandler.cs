using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateSeats;

public class UpdateSeatsHandler(
    IValidator<UpdateSeatsCommand> validator,
    IVenuesRepository repository)
    : ICommandHandler<VenueDto, UpdateSeatsCommand>
{
    public async Task<Result<VenueDto, ErrorList>> Handle(
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
            var seatResult = Seat.Create(venueResult.Value,
                seatDto.SeatNumber, seatDto.RowNumber);
            if (seatResult.IsFailure)
                return seatResult.Error.ToErrors();
            
            seats.Add(seatResult.Value);
        }

        venueResult.Value.UpdateSeats(seats);
        await repository.SaveAsync(cancellationToken);
        
        return VenueDto.FromDomainEntity(venueResult.Value);
    }
}