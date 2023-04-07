namespace Platform.Validation.Fluent.Messages
{
    public record OptionValidationMessage(string? Value) : ITelegramValidationMessage
    {
    }
}