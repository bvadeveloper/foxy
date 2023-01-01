using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Platform.Host
{
    public static class Application
    {
        private static readonly Type[] DefaultStartups =
        {
            typeof(Versioning.Startup),
            typeof(Tracing.Startup),
            typeof(Logging.Host.Startup),
            typeof(Startup),
        };

        public static void Run(string[] args, params Type[] startups)
        {
            using var host = CreateHostBuilder(args, startups).Build();
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, params Type[] startups)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    builder =>
                    {
                        builder.UseKestrel();
                        builder.UseContentRoot(Directory.GetCurrentDirectory());
                        builder.ConfigureAppConfiguration(
                            configurationBuilder =>
                            {
                                configurationBuilder
                                    .SetBasePath(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName)
                                    .AddJsonFile("tools.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables();
                            });
                        builder.UseCompositeStartup(DefaultStartups.Concat(startups).ToArray());

                        // The UseCompositeStartup extension in the method webBuilder.Configure() sets incorrect application key relatively app's entry point
                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.1#application-key-name
                        // We reinitialize the application key to the correct app entry point
                        builder.UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly()?.GetName().Name);
                    });
        }
    }
}