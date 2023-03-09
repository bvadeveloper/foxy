using System.Net;
using System.Net.Sockets;

namespace Platform.Geolocation.HostGeolocation.Stun;

public static class StunClientTcp
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

        using var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Any, 0));
        var requestBytes = request.Serialize();
        await tcpClient.ConnectAsync(stunServer.Host, stunServer.Port);
        await tcpClient.GetStream().WriteAsync(requestBytes, 0, requestBytes.Length);
        var stream = tcpClient.GetStream();

        return new StunMessage(stream);
    }
}