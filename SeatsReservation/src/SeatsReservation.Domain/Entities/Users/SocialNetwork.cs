namespace SeatsReservation.Domain.Entities.Users;

public record SocialNetwork{
    public SocialNetwork()
    {
    }

    public string Name { get; init; }

    public string Link { get; init; }
}