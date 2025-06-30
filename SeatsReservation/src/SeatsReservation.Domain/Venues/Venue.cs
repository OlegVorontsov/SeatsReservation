using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Venues;

public class Venue
{
    public Guid Id { get; set; }

    public string Name { get; private set; }
    
    public int SeatsLimit { get; private set; }
    
    private List<Seat> _seats = [];

    public IReadOnlyList<Seat> Seats => _seats;
    
    public int SeatsCount => _seats.Count;

    public Venue(Guid id, string name, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
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