using Microsoft.Extensions.DependencyInjection;

namespace SeatsReservation.Infrastructure.Postgres.Seeding;

public static class SeedingExtensions
{
    public static async Task<IServiceProvider> RunSeedingAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        
        var seeders = scope.ServiceProvider.GetServices<ISeeder>();
        
        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
        
        return services;
    }
}