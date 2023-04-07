using System.Collections.Generic;
using System.Linq;
using Platform.Contract.Profiles.Enums;

namespace Platform.Processor.Coordinator.Strategies;

internal class StrategyFactory : IStrategyFactory
{
    public IEnumerable<IProcessingStrategy> Strategies { get; init; }

    public StrategyFactory(IEnumerable<IProcessingStrategy> strategies) => Strategies = strategies;

    public IProcessingStrategy Build(ProcessingTypes processingType) => Strategies.First(strategy => strategy.ProcessingType == processingType);
}