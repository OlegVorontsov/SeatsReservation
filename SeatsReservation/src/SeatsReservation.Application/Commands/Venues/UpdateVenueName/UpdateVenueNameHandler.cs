using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueName;

public class UpdateVenueNameHandler(
    IValidator<UpdateVenueNameCommand> validator,
    IVenuesRepository repository)
    : ICommandHandler<Guid, UpdateVenueNameCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateVenueNameCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var venueId = Id<Venue>.Create(command.Id);
        
        var venueResult = await repository.GetById(venueId, cancellationToken);
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();
        
        var updateResult = venueResult.Value.UpdateName(command.Name);
        if (updateResult.IsFailure)
            return updateResult.Error.ToErrors();

        await repository.SaveAsync(cancellationToken);
        
        return venueId.Value;
    }
}