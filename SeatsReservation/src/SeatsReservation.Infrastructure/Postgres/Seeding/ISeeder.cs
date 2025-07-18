namespace SeatsReservation.Infrastructure.Postgres.Seeding;

public interface ISeeder
{
    Task SeedAsync();
}