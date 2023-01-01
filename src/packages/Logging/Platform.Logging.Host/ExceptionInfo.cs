using System;

namespace Platform.Logging.Host
{
    internal class ExceptionInfo
    {
        public static readonly ExceptionInfo Empty = new(string.Empty, string.Empty, string.Empty, null);

        private static readonly ExceptionInfo Null = new(null, null, null, null);

        public ExceptionInfo(Exception e, bool root = true)
        {
            var defaultInner = root ? Null : null;
            Message = e.Message;
            Type = e.GetType().ToString();
            StackTrace = e.StackTrace;
            InnerException = e.InnerException == null ? defaultInner : new ExceptionInfo(e.InnerException, false);
        }

        private ExceptionInfo(string? message, string? type, string? stackTrace, ExceptionInfo? innerException)
        {
            Message = message;
            Type = type;
            StackTrace = stackTrace;
            InnerException = innerException;
        }
        
        public string? Message { get; }
        
        public string? Type { get; }
        
        public string? StackTrace { get; }
        
        public ExceptionInfo? InnerException { get; }
    }
}
