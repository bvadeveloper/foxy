using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Processors;
using Platform.Processor.Coordinator.Clients;
using Platform.Services.Processor;

namespace Platform.Processor.Coordinator.Strategies;

public class EmailProcessingStrategy : IProcessingStrategy
{
    private readonly IProcessorClient _processorClient;
    private readonly ILogger _logger;

    public EmailProcessingStrategy(IProcessorClient processorClient, ILogger<EmailProcessingStrategy> logger)
    {
        _processorClient = processorClient;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Email;

    public async Task Run(CoordinatorProfile profile)
    {
        var emailProfile = new EmailProfile(profile.Target, ProcessingType);
        await _processorClient.SendToEmailParser(emailProfile);
    }
}