using System.Collections.Generic;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;

namespace Platform.Processor.Coordinator.Strategies;

public interface IStrategyFactory
{
    IEnumerable<IProcessingStrategy> Strategies { get; init; }

    IProcessingStrategy Build(ProcessingTypes type);
}