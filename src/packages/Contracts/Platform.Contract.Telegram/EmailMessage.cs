namespace Platform.Contract.Telegram
{
    public record EmailMessage(string Value) : ITelegramMessageValidation
    {
    }
}