namespace Platform.Logging
{
    /// <summary>
    /// It enables you to dynamically modify minimum logging level.
    /// </summary>
    public interface ILoggingLevel
    {
        /// <summary>
        /// Sets minimum log level to trace.
        /// </summary>
        void Trace();

        /// <summary>
        /// Sets minimum log level to debug.
        /// </summary>
        void Debug();

        /// <summary>
        /// Sets minimum log level to information.
        /// </summary>
        void Info();

        /// <summary>
        /// Sets minimum log level to warning.
        /// </summary>
        void Warn();

        /// <summary>
        /// Sets minimum log level to error.
        /// </summary>
        void Error();
    }
}