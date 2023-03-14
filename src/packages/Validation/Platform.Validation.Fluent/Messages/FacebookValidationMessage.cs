namespace Platform.Validation.Fluent.Messages
{
    public record FacebookValidationMessage(string Value) : ITelegramValidationMessage
    {
    }
}