using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Application.Shared.DTOs;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenue;

public class UpdateVenueHandler(
    IValidator<UpdateVenueCommand> validator,
    IVenuesRepository repository)
    : ICommandHandler<VenueDto, UpdateVenueCommand>
{
    public async Task<Result<VenueDto, ErrorList>> Handle(
        UpdateVenueCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        // лучше получать сущность из бд и менять что нужно
        var venueResult = Venue.Create(
            command.Name, command.Prefix, command.SeatsLimit, Id<Venue>.Create(command.Id));
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();
        
        await repository.UpdateAsync(venueResult.Value, cancellationToken);
        
        return VenueDto.FromDomainEntity(venueResult.Value);
    }
}