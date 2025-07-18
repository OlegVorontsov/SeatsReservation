namespace SeatsReservation.Domain.Entities.Users;

public record UserDetails
{
    public UserDetails()
    {

    }

    public string Description { get; set; }

    public string FIO { get; set; }

    public IReadOnlyList<SocialNetwork> Socials { get; set; }
}