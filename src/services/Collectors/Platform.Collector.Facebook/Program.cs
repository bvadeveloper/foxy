﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Host;
using Platform.Services.Collector;

namespace Platform.Collector.Facebook;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddSubscriptions(configuration, ProcessingTypes.Facebook, ExchangeTypes.Facebook)
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookScanner>();
        });
}