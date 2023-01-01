using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Platform.Logging.Extensions
{
    public static partial class LoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(this ILogger logger, string message) => logger.LogWarning(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(this ILogger logger, string message, Exception exception) => logger.LogWarning(exception, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(this ILogger logger, string message, string propertyName, object propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogWarning(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(this ILogger logger, string message, Exception exception, string propertyName, object propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogWarning(exception, message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(this ILogger logger, string message, params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogWarning(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(
            this ILogger logger,
            string message,
            Exception exception,
            params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogWarning(exception, message);
            }
        }

    }
}