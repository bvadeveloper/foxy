using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Bus.Rmq.Configurations;
using Platform.Contract;
using Platform.Contract.Models;
using RabbitMQ.Client;

namespace Platform.Bus.Rmq
{
    public static class BusExtensions
    {
        /// <summary>
        /// Just like connections, channels are meant to be long-lived. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRmq(this IServiceCollection services) =>
            services.AddSingleton<IConnection>(provider =>
            {
                var configuration = provider.GetRequiredService<IOptions<BusConfiguration>>().Value;
                var factory = new ConnectionFactory
                {
                    ClientProvidedName = AppDomain.CurrentDomain.FriendlyName.ToLower(),
                    UserName = configuration.UserName,
                    Password = configuration.Password,
                    VirtualHost = configuration.VirtualHost,
                    HostName = configuration.Host,
                    Port = configuration.Port, // TODO: switch to TLS https://www.rabbitmq.com/ssl.html
                    RequestedHeartbeat = TimeSpan.FromSeconds(10),
                    DispatchConsumersAsync = true
                };

                return factory.CreateConnection();
            }).AddScoped<IModel>(provider =>
            {
                var connection = provider.GetRequiredService<IConnection>();
                return connection.CreateModel();
            });

        public static IServiceCollection AddExchangeTypes(this IServiceCollection services, params Type[] types)
        {
            var mapping = types.Select(type =>
            {
                var attr = Attribute.GetCustomAttribute(type, typeof(ExchangeAttribute), true) as ExchangeAttribute;
                return (attr.Exchange, attr.Route);
            }).ToImmutableList();

            return services.AddSingleton(new Exchanges(mapping));
        }
    }
}