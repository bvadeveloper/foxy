using Microsoft.Extensions.DependencyInjection;
using Platform.Validation.Fluent.Abstractions;

namespace Platform.Validation.Fluent
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddSingleton<IValidationFactory, ValidationFactory>();
    }
}