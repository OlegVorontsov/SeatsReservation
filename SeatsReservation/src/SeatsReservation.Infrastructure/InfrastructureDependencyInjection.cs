using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatsReservation.Application.Interfaces.Repositories;
using SeatsReservation.Infrastructure.Postgres.Factories;
using SeatsReservation.Infrastructure.Postgres.Interfaces;
using SeatsReservation.Infrastructure.Postgres.Repositories;
using SeatsReservation.Infrastructure.Postgres.Write;
using SharedService.Core.Database.Read;

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

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        //services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();
        services.AddScoped<IVenuesRepository, NpgSqlVenuesesRepository>();

        //services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }
}