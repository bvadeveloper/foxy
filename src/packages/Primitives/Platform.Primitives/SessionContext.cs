namespace Platform.Primitives
{
    public record SessionContext
    {
        public string CorrelationId { get; set; }

        public string SessionId { get; set; }

        public static SessionContext Init() => new() { CorrelationId = Ulid.NewUlid().ToString() };

        public override string ToString() => $"{CorrelationId}:{SessionId}";
    }
}