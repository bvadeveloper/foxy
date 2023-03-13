using System;
using System.Threading.Tasks;
using MemoryPack;
using Platform.Contract.Profiles;
using static Force.Crc32.Crc32CAlgorithm;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static async ValueTask PublishToTelegramExchange(this IBusPublisher publisher, ReportProfile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Telegram));

    public static async ValueTask PublishToDomainExchange(this IBusPublisher publisher, DomainProfile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Domain));

    public static async ValueTask PublishToHostExchange(this IBusPublisher publisher, HostProfile profile, string route) =>
        await publisher.PublishToExchange(profile, Exchange.Make(ExchangeTypes.Host, route));

    public static async ValueTask PublishToReportExchange(this IBusPublisher publisher, IProfile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Report));

    public static async ValueTask PublishToCoordinatorExchange(this IBusPublisher publisher, CoordinatorProfile profile) =>
        await publisher.PublishToExchange(profile, Exchange.Default(ExchangeTypes.Coordinator));

    public static async ValueTask PublishToSynchronizationExchange(this IBusPublisher publisher, string route, string hostId)
    {
        var exchange = Exchange.Default(ExchangeTypes.Synchronization);
        var profile = new SynchronizationProfile(route, hostId);

        await publisher.PublishToExchange(profile, exchange);
    }

    private static async ValueTask PublishToExchange(this IBusPublisher publisher, IProfile profile, Exchange exchange)
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