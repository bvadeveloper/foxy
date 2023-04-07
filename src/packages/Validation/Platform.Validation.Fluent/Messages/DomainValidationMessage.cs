namespace Platform.Validation.Fluent.Messages
{
    public record DomainValidationMessage(string[]? Value) : ITelegramValidationMessage
    {
    }
}