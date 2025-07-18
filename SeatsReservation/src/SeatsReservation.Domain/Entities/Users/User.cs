namespace SeatsReservation.Domain.Entities.Users;

public class User
{
    public User()
    {

    }

    public Guid Id { get; set; }
    public UserDetails Details { get; set; }
}