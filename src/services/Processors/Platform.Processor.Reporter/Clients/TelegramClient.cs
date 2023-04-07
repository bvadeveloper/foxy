using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Contract.Profiles.Extensions;
using Platform.Contract.Profiles.Processors;
using Platform.Logging.Extensions;

namespace Platform.Processor.Reporter.Clients;

public class TelegramClient : ITelegramClient
{
    private readonly IBusPublisher _publishClient;
    private readonly ILogger _logger;

    public TelegramClient(IBusPublisher publishClient, ILogger<TelegramClient> logger)
    {
        _publishClient = publishClient;
        _logger = logger;
    }

    /// <summary>
    /// Send report profile to telegram exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToTelegram(ReportProfile profile)
    {
        var payload = profile.ToBytes();

        await _publishClient.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Telegram));
        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Telegram}' exchange, payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }
}