using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatsReservation.Application.Commands.Events;
using SeatsReservation.Application.Commands.Reservations;
using SeatsReservation.Application.Commands.Seats;
using SeatsReservation.Application.Commands.Venues;
using SeatsReservation.Application.Interfaces.Database;
using SeatsReservation.Infrastructure.Postgres.Factories;
using SeatsReservation.Infrastructure.Postgres.Repositories;
using SeatsReservation.Infrastructure.Postgres.Seeding;
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
        services.AddScoped<IReadDbContext, ApplicationWriteDbContext>(_ => new ApplicationWriteDbContext(dbConnectionString));

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        // для маппинга SnakeCase бд в CamelCase, используемый Dapper
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddScoped<ITransactionManager, TransactionManager>();
        
        //services.AddScoped<IVenuesRepository, NpgSqlVenuesRepository>();
        services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();
        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();
        services.AddScoped<ISeatsRepository, SeatsRepository>();

        services.AddScoped<ISeeder, ReservationSeeder>();

        //services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }
}