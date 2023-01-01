using System;
using System.Collections.Generic;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Bus.EasyNetQ.Configurations;

namespace Platform.Bus.EasyNetQ
{
    public static class BusExtensions
    {
        public static IServiceCollection AddBus(this IServiceCollection services) =>
            services.RegisterEasyNetQ(resolver =>
            {
                var busConfiguration = resolver.Resolve<IOptions<BusConfiguration>>().Value;

                return new ConnectionConfiguration
                {
                    Hosts = new List<HostConfiguration>
                    {
                        new()
                        {
                            Host = busConfiguration.Host,
                            Port = (ushort)busConfiguration.Port
                        }
                    },
                    UserName = busConfiguration.UserName,
                    Password = busConfiguration.Password,
                    VirtualHost = busConfiguration.VirtualHost,
                    PrefetchCount = (ushort)busConfiguration.PrefetchCount,
                    Timeout = TimeSpan.FromSeconds(busConfiguration.Timeout)
                };
            });
    }
}