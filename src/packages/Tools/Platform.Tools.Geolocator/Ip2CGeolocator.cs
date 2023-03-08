using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Tools.Geolocator;

public class Ip2CGeolocator : IGeolocator
{
    private const string Ip2CUrl = "https://ip2c.org/";

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger _logger;

    public Ip2CGeolocator(IHttpClientFactory clientFactory, ILogger<Ip2CGeolocator> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Response example 1;AU;AUS;Australia
    /// See documentation https://about.ip2c.org/#outputs
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public async Task<string> Find(IPAddress ipAddress)
    {
        try
        {
            var response = await CallEndpoint($"{Ip2CUrl}{ipAddress}");
            var responseArray = response.Split(";");

            if (responseArray[0] == "0" || responseArray[0] == "2")
            {
                _logger.Warn($"Seems {nameof(Ip2CGeolocator)} can't find a geo marker for '{ipAddress}', response is '{response}'");
                return string.Empty;
            }

            return responseArray[1].Trim().ToLower();
        }
        catch (Exception e)
        {
            _logger.Error($"External http call to endpoint '{ipAddress}' throw an exception '{e.Message}'", e);
            return string.Empty;
        }
    }

    private async Task<string> CallEndpoint(string endpoint)
    {
        using var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(endpoint);
        return await response.Content.ReadAsStringAsync();
    }
}