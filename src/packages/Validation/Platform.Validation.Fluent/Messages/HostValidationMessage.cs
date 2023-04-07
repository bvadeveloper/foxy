namespace Platform.Validation.Fluent.Messages
{
    public record HostValidationMessage(string[]? Value) : ITelegramValidationMessage
    {
    }
}