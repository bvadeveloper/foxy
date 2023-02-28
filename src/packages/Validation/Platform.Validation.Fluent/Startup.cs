using Microsoft.Extensions.DependencyInjection;

namespace Platform.Validation.Fluent
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddSingleton<IValidationFactory, ValidationFactory>();
    }
}