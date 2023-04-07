using System.Threading.Tasks;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles.Processors;
using Platform.Processor.Coordinator.Strategies;

namespace Platform.Processor.Coordinator.Processors;

public class CoordinatorProcessor : IConsumeAsync<CoordinatorProfile>
{
    private readonly IStrategyFactory _strategyFactory;

    public CoordinatorProcessor(IStrategyFactory strategyFactory) => _strategyFactory = strategyFactory;

    public async Task ConsumeAsync(CoordinatorProfile profile) => await _strategyFactory.Build(profile.ProcessingType).Run(profile);
}