using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Platform.Logging.Extensions
{
    public static partial class LoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this ILogger logger, string message) => logger.LogDebug(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this ILogger logger, string message, Exception exception) => logger.LogDebug(exception, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this ILogger logger, string message, string propertyName, object propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogDebug(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this ILogger logger, string message, Exception exception, string propertyName, object propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogDebug(exception, message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this ILogger logger, string message, params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogDebug(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(
            this ILogger logger,
            string message,
            Exception exception,
            params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogDebug(exception, message);
            }
        }

    }
}