using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Processor.Coordinator.Strategies;

public interface IProcessingStrategy
{
    ProcessingTypes ProcessingType { get; init; }
    
    Task Run(Profile profile);
}