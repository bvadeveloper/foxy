using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;
using Platform.Primitives;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static ValueTask PublishToCoordinatorExchange(this IPublisher publisher, string message) =>
        publisher.PublishTo(ExchangeTypes.GeoCoordinator, message);

    private static async ValueTask PublishTo(this IPublisher publisher, ExchangeTypes exchangeTypes, string message)
    {
        var exchange = new Exchange(exchangeTypes);
        var profile = new Profile(message);
        var payload = MemoryPackSerializer.Serialize(profile);

        await publisher.Publish(payload, exchange);
    }
}