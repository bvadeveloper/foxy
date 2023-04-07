namespace Platform.Validation.Fluent.Messages
{
    public record EmailValidationMessage(string Value) : ITelegramValidationMessage
    {
    }
}