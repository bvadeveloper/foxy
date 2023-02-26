using Platform.Contract.Enums;

namespace Platform.Contract.Abstractions;

[Exchange(ExchangeTypes.Coordinator)]
public interface ICoordinatorExchange : IExchange
{
}