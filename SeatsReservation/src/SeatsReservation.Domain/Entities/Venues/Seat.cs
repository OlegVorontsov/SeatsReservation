using CSharpFunctionalExtensions;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Entities.Venues;

public class Seat
{
    public Id<Seat> Id { get; set; } = null!;

    public int SeatNumber { get; private set; }

    public int RowNumber { get; private set; }
    
    public Id<Venue> VenueId { get; private set; } = null!;
    
    //Ef Core
    private Seat() { }

    private Seat(Id<Seat> id, int seatNumber, int rowNumber)
    {
        Id = id;
        SeatNumber = seatNumber;
        RowNumber = rowNumber;
    }

    public static Result<Seat, Error> Create(int seatNumber, int rowNumber)
    {
        if (seatNumber <= 0 || rowNumber <= 0)
            return Error.Validation("seat.number", "Row number and seat number must be greater than zero");
        
        return new Seat(Id<Seat>.Create(Guid.NewGuid()), seatNumber, rowNumber);
    }
}