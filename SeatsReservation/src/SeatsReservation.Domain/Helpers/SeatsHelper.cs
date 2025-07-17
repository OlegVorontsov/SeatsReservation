using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Domain.Helpers;

public static class SeatsHelper
{
    public static List<Seat> FindAdjacentSeatsInPreferredRow(
        IReadOnlyList<Seat> availableSeats,
        int requiredSeatsCount,
        int preferredRowNumber)
    {
        if (requiredSeatsCount <= 0) return [];
        
        var seatsInRow = availableSeats
            .Where(s => s.RowNumber == preferredRowNumber)
            .OrderBy(s => s.SeatNumber)
            .ToList();

        return FindAdjacentSeatsInRow(seatsInRow, requiredSeatsCount);
    }

    // вариант без указания preferredRowNumber
    public static List<Seat> FindBestAdjacentSeats(
        IReadOnlyList<Seat> availableSeats,
        int requiredSeatsCount)
    {
        if (requiredSeatsCount <= 0 || availableSeats.Count < requiredSeatsCount) return [];
        
        var groupedByRow = availableSeats.GroupBy(s => s.RowNumber);

        foreach (var row in groupedByRow.OrderBy(g => g.Key))
        {
            var seatsInRow = row.OrderBy(s => s.SeatNumber).ToList();
            
            var adjacentSeats = FindAdjacentSeatsInRow(seatsInRow, requiredSeatsCount);
            
            if (adjacentSeats.Count == requiredSeatsCount) return adjacentSeats;
        }
        return [];
    }

    private static List<Seat> FindAdjacentSeatsInRow(
        List<Seat> seatsInRow, int requiredSeatsCount)
    {
        if (seatsInRow.Count < requiredSeatsCount) return [];

        for (int i = 0; i <= seatsInRow.Count - requiredSeatsCount; i++)
        {
            var candidateSeats = new List<Seat>();
            var isAdjacent = true;
            
            for (int j = 0; j < requiredSeatsCount; j++)
            {
                var currentSeat = seatsInRow[i + j];
                candidateSeats.Add(currentSeat);

                if (j > 0)
                {
                    var previousSeat = seatsInRow[i + j -1];
                    if (currentSeat.SeatNumber != previousSeat.SeatNumber + 1)
                    {
                        isAdjacent = false;
                        break;
                    }
                }
            }
            
            if (isAdjacent) return candidateSeats;
        }
        return [];
    }
}