using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Processors;
using Platform.Processor.Coordinator.Clients;
using Platform.Services.Processor;

namespace Platform.Processor.Coordinator.Strategies;

public class FacebookProcessingStrategy : IProcessingStrategy
{
    private readonly ICollectorClient _processorClient;
    private readonly ILogger _logger;

    public FacebookProcessingStrategy(ICollectorClient processorClient, ILogger<FacebookProcessingStrategy> logger)
    {
        _processorClient = processorClient;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Facebook;

    public async Task Run(CoordinatorProfile profile)
    {
        var fbProfile = new FacebookProfile(profile.Target, profile.ProcessingType);
        await _processorClient.SendToFacebookParser(fbProfile);
    }
}