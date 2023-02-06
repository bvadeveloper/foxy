namespace Platform.Bus.Rmq.Configurations
{
    public record BusConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int PrefetchCount { get; set; }
        public int Timeout { get; set; }
    }
}