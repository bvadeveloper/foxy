namespace Platform.Contract.Telegram
{
    public record IpMessage(string Name) : IValidationMessage
    {
        public string Name { get; set; } = Name;
    }
}