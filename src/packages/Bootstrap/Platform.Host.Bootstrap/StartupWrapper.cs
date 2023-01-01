using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host.Bootstrap
{
    public sealed class StartupWrapper
    {
        private readonly object? _instance;
        private readonly MethodInfo? _configureMethod;
        private readonly MethodInfo? _configureServicesMethod;

        public StartupWrapper(object? instance)
        {
            if (instance != null)
            {
                var startupType = instance.GetType();

                _instance = instance;
                _configureMethod = startupType.GetMethod("Configure", BindingFlags.Instance | BindingFlags.Public);
                _configureServicesMethod = startupType.GetMethod("ConfigureServices", BindingFlags.Instance | BindingFlags.Public);
            }
        }

        public void Configure(IApplicationBuilder builder)
        {
            if (_configureMethod == null)
            {
                return;
            }

            // Create a scope for Configure, this allows creating scoped dependencies without the hassle of manually creating a scope.
            using var scope = builder.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var parameterInfos = _configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (var index = 0; index < parameterInfos.Length; index++)
            {
                var parameterInfo = parameterInfos[index];
                if (parameterInfo.ParameterType == typeof(IApplicationBuilder))
                {
                    parameters[index] = builder;
                }
                else
                {
                    try
                    {
                        parameters[index] = serviceProvider.GetRequiredService(parameterInfo.ParameterType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            $"Could not resolve a service of type '{parameterInfo.ParameterType.FullName}' " +
                            $"for the parameter '{parameterInfo.Name}' of method '{_configureMethod.Name}' on type '{_configureMethod.DeclaringType?.FullName}'.",
                            ex);
                    }
                }
            }

            _configureMethod.Invoke(_instance, BindingFlags.DoNotWrapExceptions, null, parameters,
                CultureInfo.InvariantCulture);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_configureServicesMethod == null)
            {
                return;
            }

            _configureServicesMethod.Invoke(_instance, BindingFlags.DoNotWrapExceptions, null, new object[] { services }, CultureInfo.InvariantCulture);
        }
    }
}