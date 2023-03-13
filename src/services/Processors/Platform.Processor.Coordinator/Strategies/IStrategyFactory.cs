using System.Collections.Generic;
using Platform.Contract.Profiles;

namespace Platform.Processor.Coordinator.Strategies;

public interface IStrategyFactory
{
    IEnumerable<IProcessingStrategy> Strategies { get; init; }

    IProcessingStrategy Build(ProcessingTypes type);
}