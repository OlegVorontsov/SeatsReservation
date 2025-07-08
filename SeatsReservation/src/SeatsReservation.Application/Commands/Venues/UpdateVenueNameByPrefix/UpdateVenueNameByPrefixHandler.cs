using CSharpFunctionalExtensions;
using FluentValidation;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Application.Commands.Venues.UpdateVenueNameByPrefix;

public class UpdateVenueNameByPrefixHandler(
    IValidator<UpdateVenueNameByPrefixCommand> validator,
    IVenuesRepository repository)
    : ICommandHandler<string, UpdateVenueNameByPrefixCommand>
{
    public async Task<Result<string, ErrorList>> Handle(
        UpdateVenueNameByPrefixCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        
        var venueNameResult = VenueName.CreateWithoutPrefix(command.Name);
        if (venueNameResult.IsFailure)
            return venueNameResult.Error.ToErrors();
        
        var updateResult = await repository.UpdateNameByPrefix(
            command.Prefix, venueNameResult.Value, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Error.ToErrors();

        return "Venue name's updated";
    }
}