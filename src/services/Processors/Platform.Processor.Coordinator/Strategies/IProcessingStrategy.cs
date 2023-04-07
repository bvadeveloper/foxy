using System.Threading.Tasks;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Processors;

namespace Platform.Processor.Coordinator.Strategies;

public interface IProcessingStrategy
{
    ProcessingTypes ProcessingType { get; init; }
    
    Task Run(CoordinatorProfile profile);
}