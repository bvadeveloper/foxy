using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;
using static Force.Crc32.Crc32CAlgorithm;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static async ValueTask PublishToTelegramExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Telegram));

    public static async ValueTask PublishToDomainExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Domain));

    public static async ValueTask PublishToHostExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Host));

    public static async ValueTask PublishToReportExchange(this IBusPublisher publisher, Profile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Report));

    public static ValueTask PublishToCoordinatorExchange(this IBusPublisher publisher, string message) =>
        publisher.PublishTo(ExchangeTypes.Coordinator, message);

    public static async ValueTask PublishToSynchronizationExchange(this IBusPublisher publisher, string route, Guid hostId)
    {
        var exchange = Exchange.Default(ExchangeTypes.Synchronization);
        var profile = new SynchronizationProfile(route, hostId.ToString());

        await publisher.PublishToExchange(profile, exchange);
    }

    private static async ValueTask PublishTo(this IBusPublisher publisher, ExchangeTypes exchangeTypes, string message)
    {
        var exchange = Exchange.Default(exchangeTypes);
        var profile = new Profile(message);

        await publisher.PublishToExchange(profile, exchange);
    }

    private static async Task PublishToExchange(this IBusPublisher publisher, IProfile profile, Exchange exchange)
    {
        var payload = MemoryPackSerializer.Serialize(profile)
            .AsMemory()
            .AddCrc32();

        await publisher.Publish(payload, exchange);
    }

    private static byte[] AddCrc32(this Memory<byte> memory)
    {
        var crcBytes = new byte[memory.Length + 4]; // magic numbers for crc32 value in payload
        memory.CopyTo(crcBytes);
        ComputeAndWriteToEnd(crcBytes);

        return crcBytes;
    }
}