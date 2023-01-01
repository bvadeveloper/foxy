using System;

namespace Platform.Primitive
{
    public class TraceContext
    {
        public Guid TraceId { get; set; }

        public long? ChatId { get; set; }

        public static TraceContext Empty() => new();

        public static TraceContext Init() => new() { TraceId = Guid.NewGuid() };
    }
}