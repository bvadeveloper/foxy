using System.Net;

namespace Platform.Contract.Telegram
{
    public record HostMessage(IPAddress[]? Value) : ITelegramMessage
    {
    }
}