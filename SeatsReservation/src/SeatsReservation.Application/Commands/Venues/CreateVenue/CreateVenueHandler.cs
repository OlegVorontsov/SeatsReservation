using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.CreateVenue;

public class CreateVenueHandler(
    IValidator<CreateVenueCommand> validator,
    IVenuesRepository repository)
    : ICommandHandler<VenueDto, CreateVenueCommand>
{
    public async Task<Result<VenueDto, ErrorList>> Handle(
        CreateVenueCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var venueResult = Venue.Create(
            command.Name, command.Prefix, command.SeatsLimit);
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();
        
        foreach (var seatDto in command.Seats)
        {
            var seatResult = Seat.Create(venueResult.Value,
                seatDto.SeatNumber, seatDto.RowNumber);
            if (seatResult.IsFailure)
                return seatResult.Error.ToErrors();
            
            venueResult.Value.AddSeat(seatResult.Value);
        }
        
        await repository.CreateAsync(venueResult.Value, cancellationToken);
        
        return VenueDto.FromDomainEntity(venueResult.Value);
    }
}