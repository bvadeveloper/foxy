namespace Platform.Contract.Telegram
{
    public record DomainMessage(string[]? Value) : ITelegramMessage
    {
    }
}