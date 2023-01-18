using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Host.Endpoint.Abstractions;
using Platform.Host.Endpoint.Configurations;
using Platform.Host.Endpoint.Services;

namespace Platform.Host.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IClientFactory, ClientFactory>();

            // Service configuration example:

            // EndpointConfiguration:0:Service1
            // EndpointConfiguration:0:https://service1-api/
            // EndpointConfiguration:1:Service2
            // EndpointConfiguration:1:https://service2-api/
            services.Configure<List<ServiceEndpointConfiguration>>(options =>
                Configuration.GetSection("EndpointConfiguration").Bind(options));
        }
    }
}