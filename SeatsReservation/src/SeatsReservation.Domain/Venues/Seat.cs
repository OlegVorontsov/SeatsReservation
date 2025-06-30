using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace SeatsReservation.Domain.Venues;

public class Seat
{
    public Guid Id { get; set; }

    public int SeatNumber { get; private set; }

    public int RowNumber { get; private set; }

    public Seat(Guid id, int seatNumber, int rowNumber)
    {
        Id = id;
        SeatNumber = seatNumber;
        RowNumber = rowNumber;
    }

    public static Result<Seat, Error> Create(int seatNumber, int rowNumber)
    {
        if (seatNumber <= 0 || rowNumber <= 0)
            return Error.Validation("seat.number", "Row number and seat number must be greater than zero");
        
        return new Seat(Guid.NewGuid(), seatNumber, rowNumber);
    }
}