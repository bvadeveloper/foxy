using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Contract.Profiles.Extensions;
using Platform.Contract.Profiles.Processors;
using Platform.Logging.Extensions;

namespace Platform.Telegram.Bot.Clients;

public class CoordinatorClient : ICoordinatorClient
{
    private readonly IBusPublisher _publishClient;
    private readonly ILogger _logger;

    public CoordinatorClient(IBusPublisher publishClient, ILogger<CoordinatorClient> logger)
    {
        _publishClient = publishClient;
        _logger = logger;
    }

    /// <summary>
    /// Send coordinator profile to coordinator exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToCoordinator(CoordinatorProfile profile)
    {
        var payload = profile.ToBytes();

        await _publishClient.Publish(payload, Exchange.Default(ExchangeNames.Coordinator));
        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Coordinator}' exchange, payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }
}