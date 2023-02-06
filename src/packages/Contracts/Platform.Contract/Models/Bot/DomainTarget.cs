using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Bot
{
    public record DomainTarget : Target, ITelegramExchange
    {
    }
}