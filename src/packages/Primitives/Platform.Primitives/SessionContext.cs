namespace Platform.Primitives
{
    public record SessionContext
    {
        public string TraceId { get; set; }

        public string ChatId { get; set; }

        public static SessionContext Init() => new() { TraceId = Guid.NewGuid().ToString() };

        public override string ToString() => $"{TraceId}:{ChatId}";
    }
}