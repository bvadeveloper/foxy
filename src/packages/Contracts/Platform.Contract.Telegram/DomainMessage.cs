using System;

namespace Platform.Contract.Telegram
{
    public record DomainMessage(Uri[]? Value) : ITelegramMessage
    {
    }
}