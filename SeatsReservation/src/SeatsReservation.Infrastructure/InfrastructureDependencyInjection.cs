using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Infrastructure.Postgres.Repositories;
using SeatsReservation.Infrastructure.Postgres.Write;

namespace SeatsReservation.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataBase(configuration);

        return services;
    }

    private static IServiceCollection AddDataBase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbConnectionString = configuration
            .GetConnectionString(ApplicationWriteDbContext.POSTGRES_CONFIGURATION) ??
                                 throw new ApplicationException("Postgres connection string not found");
        services.AddScoped(_ => new ApplicationWriteDbContext(dbConnectionString));

        //services.AddSingleton<IDBConnectionFactory>(_ => new ReadDBConnectionFactory(dbConnectionString));

        services.AddScoped<IVenueRepository, VenueRepository>();

        //services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }
}