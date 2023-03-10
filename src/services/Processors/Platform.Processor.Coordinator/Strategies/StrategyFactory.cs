using System.Collections.Generic;
using System.Linq;

namespace Platform.Processor.Coordinator.Strategies;

internal class StrategyFactory : IStrategyFactory
{
    public IEnumerable<IProcessingStrategy> Strategies { get; init; }

    public StrategyFactory(IEnumerable<IProcessingStrategy> strategies) => Strategies = strategies;

    public IProcessingStrategy Build(ProcessingTypes type) => Strategies.First(strategy => strategy.ProcessingType == type);
}