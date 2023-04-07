using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Extensions;
using Platform.Logging.Extensions;
using Platform.Processor.Coordinator.Repositories;

namespace Platform.Processor.Coordinator.Clients;

public class ProcessorClient : IProcessorClient
{
    private readonly ICollectorInfoRepository _collectorInfoRepository;
    private readonly IBusPublisher _publishClient;
    private readonly ILogger _logger;

    public ProcessorClient(IBusPublisher publishClient, ICollectorInfoRepository collectorInfoRepository, ILogger<ProcessorClient> logger)
    {
        _collectorInfoRepository = collectorInfoRepository;
        _publishClient = publishClient;
        _logger = logger;
    }

    /// <summary>
    /// Send domain profile to domain exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToDomainScanner(DomainProfile profile)
    {
        var payload = profile.ToBytes();
        var location = profile.IpLocations.Location;
        
        var (publicKey, route) = await _collectorInfoRepository.FindByCountryCodeOrAny(profile.ProcessingType, location);
        await _publishClient.Publish(payload, Exchange.Make(ExchangeNames.Domain, route), publicKey);

        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Domain}' exchange with route '{route}' and location '{location}', payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }

    /// <summary>
    /// Send host profile to host exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToHostScanner(HostProfile profile)
    {
        var payload = profile.ToBytes();
        var location = profile.IpLocations.Location;
        
        var (publicKey, route) = await _collectorInfoRepository.FindByCountryCodeOrAny(profile.ProcessingType, location);
        await _publishClient.Publish(payload, Exchange.Make(ExchangeNames.Host, route), publicKey);

        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Host}' exchange with route '{route}' and location '{location}', payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }

    /// <summary>
    /// Send email profile to email exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToEmailParser(EmailProfile profile)
    {
        var payload = profile.ToBytes();
        
        var (publicKey, route) = await _collectorInfoRepository.FindAny(profile.ProcessingType);
        await _publishClient.Publish(payload, Exchange.Make(ExchangeNames.Email, route), publicKey);

        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Email}' exchange with route '{route}', payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }

    /// <summary>
    /// Send facebook profile to facebook exchange
    /// </summary>
    /// <param name="profile"></param>
    public async ValueTask SendToFacebookParser(FacebookProfile profile)
    {
        var payload = profile.ToBytes();
        
        var (publicKey, route) = await _collectorInfoRepository.FindAny(profile.ProcessingType);
        await _publishClient.Publish(payload, Exchange.Make(ExchangeNames.Facebook, route), publicKey);

        _logger.Info($"Sent payload with target '{profile.Target}' to '{ExchangeNames.Facebook}' exchange with route '{route}', payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }
    

}