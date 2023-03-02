using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static async ValueTask PublishToBotExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, new Exchange(ExchangeTypes.Telegram));

    public static async ValueTask PublishToCollectorExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, new Exchange(ExchangeTypes.Collector));

    public static async ValueTask PublishToScannerExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, new Exchange(ExchangeTypes.Scanner));

    public static async ValueTask PublishToReportExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, new Exchange(ExchangeTypes.Report));

    public static ValueTask PublishToCoordinatorExchange(this IBusPublisher publisher, string message) =>
        publisher.PublishTo(ExchangeTypes.GeoCoordinator, message);

    private static async ValueTask PublishTo(this IBusPublisher publisher, ExchangeTypes exchangeTypes, string message)
    {
        var exchange = new Exchange(exchangeTypes);
        var profile = new Profile(message);

        await publisher.PublishToExchange(profile, exchange);
    }

    private static async Task PublishToExchange(this IBusPublisher publisher, Profile profile, Exchange exchange)
    {
        var payload = MemoryPackSerializer.Serialize((IProfile)profile);
        await publisher.Publish(payload, exchange);
    }
}