using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host.Bootstrap.Abstractions
{
    public interface ICompositeStartup
    {
        void Configure(IApplicationBuilder app);

        void ConfigureServices(IServiceCollection services);
    }
}