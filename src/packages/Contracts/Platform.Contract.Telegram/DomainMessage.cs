namespace Platform.Contract.Telegram
{
    public record DomainMessage(string Name) : IValidationMessage
    {
        public string Name { get; set; } = Name;
    }
}