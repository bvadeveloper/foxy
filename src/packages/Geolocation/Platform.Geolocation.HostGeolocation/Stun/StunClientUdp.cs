using System.Net.Sockets;

namespace Platform.Geolocation.HostGeolocation.Stun;

public class StunClientUdp
{
    public static async Task<StunMessage> SendRequest(StunMessage request, string stunServer) =>
        await SendRequest(request, new Uri(stunServer));

    private static async Task<StunMessage> SendRequest(StunMessage request, Uri stunServer)
    {
        if (stunServer.Scheme == "stuns")
        {
            throw new NotImplementedException("STUN secure is not supported");
        }

        if (stunServer.Scheme != "stun")
        {
            throw new ArgumentException("URI must have stun scheme", nameof(stunServer));
        }

        using var udp = new UdpClient(stunServer.Host, stunServer.Port);
        var requestBytes = request.Serialize();
        var byteCount = await udp.SendAsync(requestBytes, requestBytes.Length);
        var result = await udp.ReceiveAsync();

        using var stream = new MemoryStream(result.Buffer);
        return new StunMessage(stream);
    }
}