using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using Platform.Tools.Geolocator;
using Platform.Tools.HostGeolocation.Stun;

namespace Platform.Tools.HostGeolocation;

public class HostGeolocation : IHostGeolocation
{
    private const string AwsCheckIpUrl = "https://checkip.amazonaws.com/";

    private readonly IHttpClientFactory _clientFactory;
    private readonly IStunClient _stunClient;
    private readonly IGeolocator _geolocator;
    private readonly ILogger _logger;

    public HostGeolocation(IStunClient stunClient, IHttpClientFactory clientFactory, IGeolocator geoProvider, ILogger<HostGeolocation> logger)
    {
        _stunClient = stunClient;
        _clientFactory = clientFactory;
        _geolocator = geoProvider;
        _logger = logger;
    }

    public async Task<string> FindCountryCode()
    {
        var ipAddress = await _stunClient.Send();

        if (Equals(ipAddress, IPAddress.None))
        {
            _logger.Info($"Hmm... The Stun client didn't return any ip address, let's use a spare http endpoint for that '{AwsCheckIpUrl}'");
            ipAddress = await RequestAwsExternalIpChecker(AwsCheckIpUrl);
        }

        return await _geolocator.FindCountryCode(ipAddress);
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
        using var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(endpoint);
        return await response.Content.ReadAsStringAsync();
    }
}