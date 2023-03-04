using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host
{
    public static class Application
    {
        private static readonly Type[] DefaultStartups =
        {
            typeof(Versioning.Startup),
            typeof(Tracing.Startup),
            typeof(Logging.Host.Startup)
        };

        public static void Run(string[] args, params Type[] startups)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel();
            builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Configuration
                .SetBasePath(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName)
                .AddJsonFile("tools.json", optional: true, reloadOnChange: true)
                .AddJsonFile("limiter.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.UseCompositeStartup(DefaultStartups.Concat(startups).ToArray());

            builder.WebHost.UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly()?.GetName().Name);

            using var app = builder.Build();

            app.Run();
        }

        public static void RunCustom(string[] args, Action<IServiceCollection, ConfigurationManager> configureAction, Action<WebApplication> applicationAction, params Type[] startups)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel();
            builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Configuration
                .SetBasePath(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName)
                .AddJsonFile("tools.json", optional: true, reloadOnChange: true)
                .AddJsonFile("limiter.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.AddHealthChecks();
            
            configureAction.Invoke(builder.Services, builder.Configuration);

            // builder.UseCompositeStartup(DefaultStartups.Concat(startups).ToArray());

            builder.WebHost.UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly()?.GetName().Name);

            using var app = builder.Build();
            app.UseHealthChecks("/status");

            applicationAction.Invoke(app);

            app.Run();
        }
    }
}