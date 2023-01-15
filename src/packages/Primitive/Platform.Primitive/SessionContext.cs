using System;

namespace Platform.Primitive
{
    public class SessionContext
    {
        public Guid TraceId { get; set; }

        public long? ChatId { get; set; }

        public static SessionContext Empty() => new();

        public static SessionContext Init() => new() { TraceId = Guid.NewGuid() };
    }
}