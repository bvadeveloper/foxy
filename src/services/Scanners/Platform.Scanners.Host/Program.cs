﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Tool.GeoService;
using Platform.Tool.GeoService.Abstractions;
using Platform.Tools.Extensions;

namespace Platform.Scanners.Host;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.Host)
                .AddTools(configuration)
                .AddScoped<IConsumeAsync<Profile>, HostScanner>()
                .AddScoped<IGeoService, GeoService>();
        });
}