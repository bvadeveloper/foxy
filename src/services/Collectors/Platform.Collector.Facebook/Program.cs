﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Geolocation.IpResolver;
using Platform.Host;
using Platform.Services.Collector;
using Platform.Tools.Extensions;

namespace Platform.Collector.Facebook;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddSubscription(configuration, ExchangeTypes.Facebook)
                .AddPublicIpResolver()
                .AddTools(configuration)
                .AddCollectorInfo(ProcessingTypes.Facebook)
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookScanner>();
        });
}