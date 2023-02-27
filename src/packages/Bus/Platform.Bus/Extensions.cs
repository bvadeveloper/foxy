using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Bus.Configurations;
using RabbitMQ.Client;

namespace Platform.Bus
{
    public static class Extensions
    {
        /// <summary>
        /// Just like connections, channels are meant to be long-lived. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRmq(this IServiceCollection services) =>
            services.AddSingleton<IConnection>(provider =>
            {
                var configuration = provider.GetRequiredService<IOptions<RmqConfiguration>>().Value;
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

        public static IServiceCollection AddRmqConfiguration(this IServiceCollection services,
            IConfiguration configuration) =>
            services.Configure<RmqConfiguration>(options =>
                configuration.GetSection("Bus").Bind(options));
    }
}