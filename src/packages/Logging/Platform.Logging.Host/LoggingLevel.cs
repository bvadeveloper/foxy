using Serilog.Core;
using Serilog.Events;

namespace Platform.Logging.Host
{
    internal class LoggingLevel : ILoggingLevel
    {
        private readonly LoggingLevelSwitch _loggingLevelSwitch;

        public LoggingLevel(LoggingLevelSwitch loggingLevelSwitch)
        {
            _loggingLevelSwitch = loggingLevelSwitch;
        }

        public void Trace()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
        }

        public void Debug()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
        }

        public void Info()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
        }

        public void Warn()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        }

        public void Error()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Error;
        }
    }
}