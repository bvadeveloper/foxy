namespace Platform.Bus.EasyNetQ.Configurations
{
    public record BusConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; } = "/";
        public int PrefetchCount { get; set; } = 10;
        public int Timeout { get; set; } = 60;
    }
}