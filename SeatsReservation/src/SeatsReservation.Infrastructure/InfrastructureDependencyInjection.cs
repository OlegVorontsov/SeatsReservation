using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatsReservation.Application.Commands.Events;
using SeatsReservation.Application.Commands.Reservations;
using SeatsReservation.Application.Commands.Seats;
using SeatsReservation.Application.Commands.Venues;
using SeatsReservation.Application.Interfaces.Database;
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
        services.AddScoped<ITransactionManager, TransactionManager>();
        
        //services.AddScoped<IVenuesRepository, NpgSqlVenuesRepository>();
        services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();
        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();
        services.AddScoped<ISeatsRepository, SeatsRepository>();

        //services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }
}