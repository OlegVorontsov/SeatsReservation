using CSharpFunctionalExtensions;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Entities.Venues;

public class Venue
{
    public Id<Venue> Id { get; } = null!;

    public VenueName Name { get; private set; } = null!;
    
    public int SeatsLimit { get; private set; }
    
    private List<Seat> _seats = [];

    public IReadOnlyList<Seat> Seats => _seats;
    
    public int SeatsCount => _seats.Count;
    
    //Ef Core
    private Venue() { }

    private Venue(Id<Venue> id, VenueName name, int seatsLimit)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
    }
    
    public static Result<Venue, Error> Create(
        string name, string prefix, int seatsLimit, Id<Venue>? venueId = null)
    {
        if (seatsLimit <= 0)
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");

        var venueNameResult = VenueName.Create(name, prefix);
        if (venueNameResult.IsFailure)
            return venueNameResult.Error;
        
        return new Venue
            (venueId ?? Id<Venue>.Create(Guid.NewGuid()),
                venueNameResult.Value,
                seatsLimit);
    }

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
            return Error.Validation("venue.seats.limit", "Seats limit has been exceeded");
        
        _seats.Add(seat);
        return UnitResult.Success<Error>();
    }
    
    public UnitResult<Error> UpdateSeats(List<Seat> seats)
    {
        if (seats.Count > SeatsLimit)
            return Error.Validation("venue.seats.limit", "Seats limit has been exceeded");
        
        _seats = seats;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateName(string name)
    {
        var venueNameResult = VenueName.Create(name, Name.Prefix);
        if (venueNameResult.IsFailure)
            return Error.Failure("create.venue.name", "Fail to create new venue name");
        
        Name = venueNameResult.Value;
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> ExpandSeatsLimit(int newSeatsLimit)
    {
        if (newSeatsLimit <= 0)
            return Error.Validation("venue.seats.limit", "New seats limit invalid");
        
        SeatsLimit = newSeatsLimit;
        return UnitResult.Success<Error>();
    }
}