using Microsoft.Extensions.DependencyInjection;

namespace Platform.Validation.Fluent
{
    public static class Extensions
    {
        public static  IServiceCollection AddValidation(this IServiceCollection services) =>
            services.AddSingleton<IValidationFactory, ValidationFactory>();
    }
}