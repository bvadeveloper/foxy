namespace Platform.Primitives
{
    public record SessionContext
    {
        public Guid TraceId { get; set; }

        public long? ChatId { get; set; }

        public static SessionContext Init() => new() { TraceId = Guid.NewGuid() };
    }
}