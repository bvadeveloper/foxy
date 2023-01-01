using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Platform.Host.Bootstrap;
using Platform.Host.Bootstrap.Abstractions;

namespace Platform.Host
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseCompositeStartup(this IWebHostBuilder webBuilder, params Type[] startups)
        {
            webBuilder.ConfigureServices((context, collection) =>
            {
                var startup = new CompositeStartup(startups, context.Configuration);
                startup.ConfigureServices(collection);
                collection.AddSingleton<ICompositeStartup>(startup);
            });
            webBuilder.Configure((context, builder) =>
            {
                var startup = builder.ApplicationServices.GetService<ICompositeStartup>();
                builder.UseHealthChecks("/status");
                startup.Configure(builder);
            });

            return webBuilder;
        }
    }
}