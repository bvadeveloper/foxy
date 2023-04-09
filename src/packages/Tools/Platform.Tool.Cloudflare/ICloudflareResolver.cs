using System.Net;

namespace Platform.Tool.Cloudflare;

public interface ICloudflareResolver
{
    ValueTask<bool> HasProtection(string url);
    
    Task<IPAddress[]> TryResolveRealIpAddresses(string url);
}