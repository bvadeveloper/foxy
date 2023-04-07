using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Host.Versioning;
using Platform.Logging.Host;
using Platform.Primitives;

namespace Platform.Host
{
    public static class Application
    {
        public static async Task RunAsync(string[] args, Action<IServiceCollection, ConfigurationManager> configureAction, Action<WebApplication>? applicationAction = default)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel();
            builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Configuration
                .SetBasePath(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName)
                .AddJsonFile("tools.json", optional: true, reloadOnChange: true)
                .AddJsonFile("limiter.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services
#if DEBUG
                .AddTransient<VersioningMiddleware>()
#endif
                .AddScoped(_ => new StrongBox<SessionContext?>(SessionContext.Init()))
                .AddScoped<SessionContext>(provider =>
                {
                    var strongBox = provider.GetService<StrongBox<SessionContext?>>();
                    return strongBox?.Value ?? SessionContext.Init();
                })
                .AddSerilogLogger()
                .AddHealthChecks();

            // invoke custom service registration
            configureAction(builder.Services, builder.Configuration);


            await using var app = builder.Build();
            app.UseHealthChecks("/status");
#if DEBUG
            app.UseMiddleware<VersioningMiddleware>();
#endif
            // invoke custom application registrations
            applicationAction?.Invoke(app);

            await app.RunAsync();
        }
    }
}