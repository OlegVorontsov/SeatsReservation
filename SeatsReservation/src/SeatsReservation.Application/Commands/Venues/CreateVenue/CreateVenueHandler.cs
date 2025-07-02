using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.CreateVenue;

public class CreateVenueHandler(
    IValidator<CreateVenueCommand> validator,
    IVenueRepository repository)
    : ICommandHandler<VenueDto, CreateVenueCommand>
{
    public async Task<Result<VenueDto, ErrorList>> Handle(
        CreateVenueCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var venueNameResult = VenueName.Create(command.Name, command.Prefix);
        if (venueNameResult.IsFailure)
            return venueNameResult.Error.ToErrors();
        
        var venue = new Venue(
            Id<Venue>.Create(Guid.NewGuid()),
            venueNameResult.Value,
            command.SeatsLimit,
            command.Seats.Select(s => new Seat(
                Id<Seat>.Create(Guid.NewGuid()),
                s.SeatNumber,
                s.RowNumber)));
        
        await repository.CreateAsync(venue, cancellationToken);
        
        return VenueDto.FromDomainEntity(venue);
    }
}