using System.Threading.Tasks;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static async ValueTask PublishToTelegramExchange(this IBusPublisher publisher, ReportProfile profile) =>
        await publisher.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Telegram));

    public static async ValueTask PublishToCoordinatorExchange(this IBusPublisher publisher, CoordinatorProfile profile) =>
        await publisher.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Coordinator));

    public static async ValueTask PublishToReportExchange(this IBusPublisher publisher, IProfile profile, byte[] publicKey) =>
        await publisher.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Report), publicKey);

    public static async ValueTask PublishToDomainExchange(this IBusPublisher publisher, DomainProfile profile, byte[] publicKey) =>
        await publisher.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Domain), publicKey);

    public static async ValueTask PublishToHostExchange(this IBusPublisher publisher, HostProfile profile, string route, byte[] publicKey) =>
        await publisher.Publish(profile.ToBytes(), Exchange.Make(ExchangeNames.Host, route), publicKey);


    public static async ValueTask PublishToSyncExchange(this IBusPublisher publisher, CollectorInfo collectorInfo, byte[] ipAddressBytes, byte[] publicKey)
    {
        var exchange = Exchange.Default(ExchangeNames.Sync);
        var profile = new SynchronizationProfile(collectorInfo, ipAddressBytes, publicKey);

        await publisher.Publish(profile.ToBytes(), exchange);
    }
}