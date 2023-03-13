using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;

namespace Platform.Processor.Coordinator.Strategies;

public class FacebookProcessingStrategy : IProcessingStrategy
{
    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Facebook;
    private readonly ILogger _logger;

    public FacebookProcessingStrategy(ILogger<FacebookProcessingStrategy> logger)
    {
        _logger = logger;
    }

    public Task Run(CoordinatorProfile profile)
    {
        throw new System.NotImplementedException();
    }
}