using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using Platform.Geolocation.HostLocation.Stun;

namespace Platform.Geolocation.HostLocation;

public class HostLocation : IHostLocation
{
    private const string AwsCheckIpUrl = "https://checkip.amazonaws.com/";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStunClient _stunClient;
    private readonly ILogger _logger;

    public HostLocation(IStunClient stunClient, IHttpClientFactory httpClientFactory, ILogger<HostLocation> logger)
    {
        _stunClient = stunClient;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IPAddress> FindPublicIpAddress()
    {
        var ipAddress = await _stunClient.Send();

        if (Equals(ipAddress, IPAddress.None))
        {
            _logger.Info($"Hmm... The Stun client didn't return any ip address, let's use a spare http endpoint for that '{AwsCheckIpUrl}'");
            ipAddress = await RequestAwsExternalIpChecker(AwsCheckIpUrl);
        }

        return ipAddress;
    }

    /// <summary>
    /// Just a second option for testing purposes
    /// </summary>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    private async Task<IPAddress> RequestAwsExternalIpChecker(string endpoint)
    {
        try
        {
            var ips = await CallEndpoint(endpoint);
            var ip = ips.Split(";")[0].Trim(); // can be more than one ip address

            return IPAddress.Parse(ip);
        }
        catch (Exception e)
        {
            _logger.Error($"External http call to endpoint '{endpoint}' throw an exception '{e.Message}'", e);
            return IPAddress.None;
        }
    }

    private async Task<string> CallEndpoint(string endpoint)
    {
        using var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(endpoint);
        return await response.Content.ReadAsStringAsync();
    }
}