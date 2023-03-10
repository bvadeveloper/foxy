using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Processor.Coordinator.Services;

namespace Platform.Processor.Coordinator.Strategies;

public class DomainProcessingStrategy : IProcessingStrategy
{
    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Domain;
    private readonly IHostResolver _hostResolver;
    private readonly ILogger _logger;

    public DomainProcessingStrategy(IHostResolver hostResolver, ILogger<DomainProcessingStrategy> logger)
    {
        _hostResolver = hostResolver;
        _logger = logger;
    }
    
    public async Task Run(Profile profile)
    {
        // resolve all host by domain name, try to get real IPs behind CloudFlare
        var ipAddressCollection = await _hostResolver.Resolve(profile.TargetName);

        throw new System.NotImplementedException();
    }
}