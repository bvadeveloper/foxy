namespace Platform.Contract.Telegram
{
    public record HostMessage(string[]? Value) : ITelegramMessageValidation
    {
    }
}