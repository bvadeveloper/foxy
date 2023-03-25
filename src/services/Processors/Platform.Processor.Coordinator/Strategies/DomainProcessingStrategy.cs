using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;

namespace Platform.Processor.Coordinator.Strategies;

public class DomainProcessingStrategy : IProcessingStrategy
{
    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Domain;
    private readonly ILogger _logger;

    public DomainProcessingStrategy(ILogger<DomainProcessingStrategy> logger)
    {
        _logger = logger;
    }
    
    public async Task Run(CoordinatorProfile profile)
    {
        // resolve all host by domain name, try to get real IPs behind CloudFlare
        // var ipAddressCollection = await _hostResolver.Resolve(profile.TargetNames);

        throw new NotImplementedException();
    }
}