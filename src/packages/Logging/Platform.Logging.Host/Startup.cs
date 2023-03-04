using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Serilog.Formatting;
using ISerilogLogger = Serilog.ILogger;

namespace Platform.Logging.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging(x => x.ClearProviders());

            AddSerilogServices(services);
            AddServiceLogProperty(services);
        }

        private static void AddServiceLogProperty(IServiceCollection services)
        {
            services.AddSingleton(
                sp =>
                {
                    var entryAssembly = Assembly.GetEntryAssembly();
                    var name = entryAssembly?.GetName().Name ?? "<unresolved>";
                    var version = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                    var service = new Dictionary<string, string>
                    {
                        ["component"] = name.Split('.').First().ToLowerInvariant(),
                        ["name"] = name,
                        ["version"] = version ?? "0.0.0.0",
                    };
                    return new LogProperty("service", service);
                });
        }

        private static void AddSerilogServices(IServiceCollection services)
        {
            RegisterSettings<LoggingSettings>(services, "Logging");

            services.AddSingleton<LoggingLevelSwitch>();
            services.AddSingleton<ILoggingLevel, LoggingLevel>();
            services.AddTransient<ITextFormatter, JsonTextFormatter>();
            services.AddSingleton<ISerilogLogger>(
                sp =>
                {
                    var settings = sp.GetService<LoggingSettings>();
                    var levelSwitch = sp.GetService<LoggingLevelSwitch>();
                    var textFormatter = sp.GetService<ITextFormatter>();
                    var sinks = sp.GetServices<ILogEventSink>();
                    var enrich = new LoggerConfiguration()
                        .SetLogLevels(levelSwitch, settings.LogLevel)
                        .Enrich;
                    var loggerConfiguration = enrich.FromLogContext();
                    var withAllSinks = sinks.Aggregate(loggerConfiguration, (current, sink) => current.WriteTo.Sink(sink));
                    var withConsoleSink = withAllSinks.WriteTo.Console(textFormatter);
                    return withConsoleSink.CreateLogger();
                });

            services.AddSingleton<ILoggerFactory, SerilogLoggerFactory>(
                sp =>
                {
                    var logger = sp.GetService<ISerilogLogger>();
                    return new SerilogLoggerFactory(logger, true);
                });
        }

        private static void RegisterSettings<TModel>(IServiceCollection services, string section)
            where TModel : class, new()
        {
            services.AddSingleton(
                c =>
                {
                    var configuration = c.GetRequiredService<IConfiguration>();
                    var configurationSection = configuration.GetSection(section);
                    var instance = new TModel();
                    configurationSection.Bind(instance);
                    return instance;
                });
        }
    }
}
