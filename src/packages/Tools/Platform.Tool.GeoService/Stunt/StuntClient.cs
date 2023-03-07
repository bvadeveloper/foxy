using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using Platform.Tool.GeoService.Stunt.Attributes;

namespace Platform.Tool.GeoService.Stunt;

public class StuntClient : IStuntClient
{
    private readonly ILogger _logger;

    private const int TimeoutMilliseconds = 1500;

    private readonly string[] _hosts =
    {
        // Google
        "stun://stun.l.google.com:19302",
        "stun://stun1.l.google.com:19302",
        "stun://stun2.l.google.com:19302",
        "stun://stun3.l.google.com:19302",
        "stun://stun4.l.google.com:19302",

        // Other
        "stun://stun.voip.blackberry.com:3478",
        "stun://stun.voipgate.com:3478",
        "stun://stun.voys.nl:3478",
        "stun://stun1.faktortel.com.au:3478",

        // TCP
        "stun://stun.sipnet.net:3478",
        "stun://stun.sipnet.ru:3478",
        "stun://stun.stunprotocol.org:3478",
    };

    public StuntClient(ILogger<StuntClient> logger) => _logger = logger;

    public async Task<IPAddress> RequestExternalIp()
    {
        var message = MakeStunMessage();

        foreach (var host in _hosts.RandomElements())
        {
            try
            {
                var response = await StunClientTcp.SendRequest(message, host).AwaitWithTimeout(TimeoutMilliseconds);

                var ipEndpoint = response.Attributes
                    .First(a => a.Type == StunAttributeType.XOR_MAPPED_ADDRESS)
                    .GetXorMappedAddress();

                _logger.Info($"The Stun tcp response for the host '{host}' is {ipEndpoint.Address}");

                return ipEndpoint.Address;
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while the Stun endpoint call '{host}'. '{e.Message}'", e);
            }
        }

        return IPAddress.None;
    }

    private static StunMessage MakeStunMessage()
    {
        var message = new StunMessage();
        var softwareAttribute = new StunAttribute(StunAttributeType.SOFTWARE, message);
        softwareAttribute.SetSoftware("foxy");
        message.Attributes.Add(softwareAttribute);

        return message;
    }
}