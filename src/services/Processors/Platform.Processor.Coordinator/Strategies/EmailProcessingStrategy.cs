using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;

namespace Platform.Processor.Coordinator.Strategies;

public class EmailProcessingStrategy : IProcessingStrategy
{
    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Email;
    private readonly ILogger _logger;

    public EmailProcessingStrategy(ILogger<EmailProcessingStrategy> logger)
    {
        _logger = logger;
    }

    public Task Run(CoordinatorProfile profile)
    {
        throw new System.NotImplementedException();
    }
}