using System.Threading.Tasks;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Processor.Coordinator.Strategies;

namespace Platform.Processor.Coordinator.Processors;

public class CoordinatorProcessor : IConsumeAsync<Profile>
{
    private readonly IStrategyFactory _strategyFactory;

    public CoordinatorProcessor(IStrategyFactory strategyFactory) => _strategyFactory = strategyFactory;

    public async ValueTask ConsumeAsync(Profile profile) => await _strategyFactory.Build(profile.TargetName.ToProcessingType()).Run(profile);
}