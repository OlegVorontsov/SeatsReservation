using Microsoft.EntityFrameworkCore;
using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Infrastructure.Postgres.Write;

public class ApplicationWriteDbContext(string connectionString) : DbContext
{
    public const string POSTGRES_CONFIGURATION = "Postgres";

    public DbSet<Venue> Venues => Set<Venue>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.LogTo(Console.WriteLine)
                      .EnableSensitiveDataLogging();
        optionsBuilder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("seats_reservation");
        
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationWriteDbContext).Assembly,
            type => type.FullName?.Contains("Postgres.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }
}