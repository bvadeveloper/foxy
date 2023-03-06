﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Tool.GeoIp;
using Platform.Tools.Extensions;

namespace Platform.Scanners.Domain;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.Domain)
                .AddTools(configuration)
                .AddScoped<IConsumeAsync<Profile>, DomainScanner>()
                .AddScoped<IGeoIpService, GeoIpService>();
        });
}