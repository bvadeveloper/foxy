using System.Collections.Generic;

namespace Platform.Processor.Coordinator.Strategies;

public interface IStrategyFactory
{
    IEnumerable<IProcessingStrategy> Strategies { get; init; }

    IProcessingStrategy Build(ProcessingTypes type);
}