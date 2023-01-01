using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Logging.Migrations
{
    public static class MigrationLoggerExtentions
    {
        public static void AddMigrationsLogger(this IServiceCollection services, IConfiguration configuration)
        {
            //  Log.Logger = new LoggerConfiguration()
            //     .ReadFrom.Configuration(configuration)
            //     .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                // builder.ClearProviders();
                // builder.AddConfiguration(configuration.GetSection("Logging"));
                // builder.AddConsole();
                // builder.AddSerilog();
            });
        }
    }
}
