using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;

namespace SeatsReservation.Infrastructure.Postgres.Write;

public class ApplicationWriteDbContext(string connectionString) : DbContext
{
    public const string POSTGRES_CONFIGURATION = "Postgres";

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
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
    
    private ILoggerFactory CreateLoggerFactory() =>
    LoggerFactory.Create(builder => {builder.AddConsole();});
}