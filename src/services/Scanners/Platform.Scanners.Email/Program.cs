using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Services;
using Platform.Tools.Extensions;
using Platform.Tools.HostGeolocation;

namespace Platform.Scanners.Email;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddScannerSubscription(configuration, ExchangeTypes.Email)
                .AddHostGeolocation()
                .AddTools(configuration)
                .AddScoped<IConsumeAsync<Profile>, EmailScanner>();
        });
}