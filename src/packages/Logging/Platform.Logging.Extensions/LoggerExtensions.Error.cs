using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Platform.Logging.Extensions
{
    public static partial class LoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this ILogger logger, string message) => logger.LogError(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this ILogger logger, string message, Exception exception) => logger.LogError(exception, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this ILogger logger, string message, string propertyName, object? propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogError(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this ILogger logger, string message, Exception exception, string propertyName, object? propertyValue)
        {
            using (logger.With((propertyName, propertyValue)))
            {
                logger.LogError(exception, message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this ILogger logger, string message, params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogError(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(
            this ILogger logger,
            string message,
            Exception exception,
            params (string propertyName, object? propertyValue)[] props)
        {
            using (logger.With(props))
            {
                logger.LogError(exception, message);
            }
        }

    }
}