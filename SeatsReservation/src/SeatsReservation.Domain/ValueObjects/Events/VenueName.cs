using CSharpFunctionalExtensions;
using SeatsReservation.Domain.Shared;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.ValueObjects.Events;

public record VenueName
{
    public string Name { get; }
    
    public string Prefix { get; }

    private VenueName(string name, string prefix)
    {
        Name = name;
        Prefix = prefix;
    }

    public override string ToString() => $"{Prefix}-{Name}";
    
    public static Result<VenueName, Error> Create(string name, string prefix)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("VenueName.name", "Name is required");

        if (string.IsNullOrWhiteSpace(prefix))
            return Error.Validation("VenueName.prefix", "Prefix is required");
        
        if (name.Length > Constants.Length._50)
            return Error.Validation("VenueName.name", "Name must be less than 50 characters");
        
        if (prefix.Length > Constants.Length._50)
            return Error.Validation("VenueName.prefix", "Prefix must be less than 50 characters");
        
        return new VenueName(name, prefix);
    }
}