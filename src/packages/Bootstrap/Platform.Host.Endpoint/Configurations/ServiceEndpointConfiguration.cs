namespace Platform.Host.Endpoint.Configurations
{
    public record ServiceEndpointConfiguration
    {
        public string ServiceName { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}