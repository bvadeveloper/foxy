using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static ValueTask PublishToCoordinatorExchange(this IBusPublisher publisher, string message) =>
        publisher.PublishTo(ExchangeTypes.GeoCoordinator, message);

    private static async ValueTask PublishTo(this IBusPublisher publisher, ExchangeTypes exchangeTypes, string message)
    {
        var exchange = new Exchange(exchangeTypes);
        var profile = new Profile(message);
        var payload = MemoryPackSerializer.Serialize((IProfile)profile);

        await publisher.Publish(payload, exchange);
    }
}