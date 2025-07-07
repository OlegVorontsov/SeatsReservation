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

    private Venue(Id<Venue> id, VenueName name, int seatsLimit, List<Seat> seats)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
        _seats = seats;
    }
    
    public static Result<Venue, Error> Create(
        string name, string prefix, int seatsLimit, List<Seat> seats)
    {
        if (seatsLimit <= 0)
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");

        var venueNameResult = VenueName.Create(name, prefix);
        if (venueNameResult.IsFailure)
            return venueNameResult.Error;
        
        if (seats.Count < 1)
            return Error.Validation("venue.seats", "Seats count must be greater than one");
        
        if (seats.Count > seatsLimit)
            return Error.Validation("venue.seats", "Seats count exceeds the venue's seat limit");
        
        return new Venue(Id<Venue>.Create(Guid.NewGuid()), venueNameResult.Value, seatsLimit, seats);
    }

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
            return Error.Validation("venue.seats.limit", "Seats limit has been exceeded");
        
        _seats.Add(seat);
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