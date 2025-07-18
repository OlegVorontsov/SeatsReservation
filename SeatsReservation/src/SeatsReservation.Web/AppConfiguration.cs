using SeatsReservation.Infrastructure.Postgres.Seeding;

namespace SeatsReservation.Web;

public static class AppConfiguration
{
    public static async Task<WebApplication> Configure(this WebApplication app, string[] args)
    {
        app.UseStaticFiles();
        //app.UseExceptionMiddleware();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "SeatsReservation");
                options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            });

            if (args.Contains("--seed"))
            {
                //await app.Services.RunSeedingAsync();
            }
            
            //await app.ApplyMigrations();
        }

        app.MapControllers();

        return app;
    }
}