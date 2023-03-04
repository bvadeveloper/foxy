using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Host.Bootstrap.Abstractions;

namespace Platform.Host.Bootstrap
{
    public class CompositeStartupBuilder : ICompositeStartup
    {
        private readonly IList<StartupWrapper> _startupWrappers = new List<StartupWrapper>();

        public CompositeStartupBuilder(IEnumerable<Type> startups, params object[] startupArguments)
        {
            foreach (var startup in startups)
            {
                var constructor = startup.GetConstructors().First();
                var ctorParameters = constructor.GetParameters();
                object? instance;
                if (ctorParameters.Any())
                {
                    var startupTypes = startupArguments.Select(sa => sa.GetType()).ToList();
                    var invalidArguments = ctorParameters.Where(
                            cp => !startupTypes.Any(st => cp.ParameterType.IsAssignableFrom(st)))
                        .ToList();

                    if (invalidArguments.Any())
                    {
                        throw new ArgumentException(
                            $"Startup class contains invalid arguments in {startup.FullName} constructor",
                            string.Join(",", invalidArguments));
                    }

                    instance = Activator.CreateInstance(startup,
                        ctorParameters.Select(
                            cp => startupArguments.First(
                                sa => cp.ParameterType.IsAssignableFrom(sa.GetType()))).ToArray());
                }
                else
                {
                    instance = Activator.CreateInstance(startup);
                }

                _startupWrappers.Add(new StartupWrapper(instance));
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            foreach (var startupWrapper in _startupWrappers)
            {
                startupWrapper.Configure(app);
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var startupWrapper in _startupWrappers)
            {
                startupWrapper.ConfigureServices(services);
            }
        }
    }
}