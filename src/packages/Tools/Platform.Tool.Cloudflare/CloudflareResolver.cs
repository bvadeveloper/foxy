using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Tool.Cloudflare;

public class CloudflareResolver : ICloudflareResolver
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger _logger;

    public CloudflareResolver(IHttpClientFactory clientFactory, ILogger<CloudflareResolver> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    private const string HostName = "cloudflare";

    public async ValueTask<bool> HasProtection(string url) =>
        await HasCloudflareCertificate(url)
        || await HasCloudflareDns(url)
        || await HasCloudflareHeaders(url);

    public Task<IPAddress[]> TryResolveRealIpAddresses(string url)
    {
        throw new NotImplementedException();
    }

    private async ValueTask<bool> HasCloudflareCertificate(string url)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(url, 443);
        var sslStream = new SslStream(tcpClient.GetStream());
        await sslStream.AuthenticateAsClientAsync(url);

        var response = sslStream.RemoteCertificate != null
                       && sslStream.RemoteCertificate
                           .Issuer
                           .ToLowerInvariant()
                           .Contains(HostName);

        _logger.Info($"Host '{url}' has Cloudflare certificate.");

        return response;
    }

    private async ValueTask<bool> HasCloudflareDns(string url)
    {
        var response = (await Dns.GetHostEntryAsync(url)).AddressList
            .Any(nameserver => nameserver
                .ToString()
                .ToLowerInvariant()
                .Contains(HostName));

        _logger.Info($"Host '{url}' has Cloudflare DNS.");
        
        return response;
    }

    private async ValueTask<bool> HasCloudflareHeaders(string url)
    {
        var httpClient = _clientFactory.CreateClient();
        var httpResponse = await httpClient.GetAsync($"https://{url}");
        var response = httpResponse.Headers.TryGetValues("Server", out var values)
                       && values.Any(value => value.ToLowerInvariant().Contains(HostName));

        _logger.Info($"Host '{url}' has Cloudflare headers.");
        
        return response;
    }
}