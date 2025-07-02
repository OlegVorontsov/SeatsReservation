using CSharpFunctionalExtensions;
using SeatsReservation.Domain.ValueObjects.Events;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Entities.Venues;

public class Venue
{
    public Id<Venue> Id { get; } = null!;

    public VenueName VenueName { get; private set; } = null!;
    
    public int SeatsLimit { get; private set; }
    
    private List<Seat> _seats = [];

    public IReadOnlyList<Seat> Seats => _seats;
    
    public int SeatsCount => _seats.Count;
    
    //Ef Core
    private Venue() { }

    public Venue(Id<Venue> id, VenueName venueName, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        VenueName = venueName;
        SeatsLimit = seatsLimit;
        _seats = seats.ToList();
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