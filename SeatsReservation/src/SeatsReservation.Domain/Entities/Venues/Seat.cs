using CSharpFunctionalExtensions;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Entities.Venues;

public class Seat
{
    public Id<Seat> Id { get; set; } = null!;

    public int SeatNumber { get; private set; }

    public int RowNumber { get; private set; }
    
    public Venue Venue { get; private set; } = null!;
    public Id<Venue> VenueId { get; private set; } = null!;
    
    //Ef Core
    private Seat() { }

    private Seat(Id<Seat> id, Venue venue, int seatNumber, int rowNumber)
    {
        Id = id;
        Venue = venue;
        SeatNumber = seatNumber;
        RowNumber = rowNumber;
    }
    
    private Seat(Id<Seat> id, Id<Venue> venueId, int seatNumber, int rowNumber)
    {
        Id = id;
        VenueId = venueId;
        SeatNumber = seatNumber;
        RowNumber = rowNumber;
    }

    public static Result<Seat, Error> Create(Venue venue, int seatNumber, int rowNumber)
    {
        if (seatNumber <= 0 || rowNumber <= 0)
            return Error.Validation("seat.number", "Row number and seat number must be greater than zero");
        
        return new Seat(Id<Seat>.Create(Guid.NewGuid()), venue, seatNumber, rowNumber);
    }
    
    public static Result<Seat, Error> Create(Id<Venue> venueId, int seatNumber, int rowNumber)
    {
        if (seatNumber <= 0 || rowNumber <= 0)
            return Error.Validation("seat.number", "Row number and seat number must be greater than zero");
        
        return new Seat(Id<Seat>.Create(Guid.NewGuid()), venueId, seatNumber, rowNumber);
    }
}