using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Platform.Logging.Host
{
    internal static class SerilogExtensions
    {
        public static LoggerConfiguration SetLogLevels(
            this LoggerConfiguration loggerConfiguration,
            LoggingLevelSwitch levelSwitch,
            IDictionary<string, LogLevel> levels) =>
            levels
                .Aggregate(
                    loggerConfiguration,
                    (current, lv) =>
                        lv.Key.Equals("Default")
                            ? current.SetLogLevel(levelSwitch, lv.Value)
                            : current.SetLogOverride(lv.Key, lv.Value));

        private static LoggerConfiguration SetLogLevel(this LoggerConfiguration logger, LoggingLevelSwitch levelSwitch, LogLevel level)
        {
            levelSwitch.MinimumLevel = level.ToSerilogLevel();
            logger.MinimumLevel.ControlledBy(levelSwitch);
            return logger;
        }

        private static LogEventLevel ToSerilogLevel(this LogLevel level) =>
            level switch
            {
                LogLevel.Critical => LogEventLevel.Fatal,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Trace => LogEventLevel.Verbose,
                _ => LogEventLevel.Information
            };

        private static LoggerConfiguration SetLogOverride(this LoggerConfiguration logger, string key, LogLevel level)
        {
            if (!string.IsNullOrEmpty(key))
            {
                logger.MinimumLevel.Override(key, level.ToSerilogLevel());
            }

            return logger;
        }
    }
}