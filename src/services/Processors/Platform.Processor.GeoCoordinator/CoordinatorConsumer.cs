using System.Threading.Tasks;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.GeoCoordinator;

public class CoordinatorConsumer : IConsumeAsync<Profile>
{
    public ValueTask ConsumeAsync(Profile profile)
    {
        throw new System.NotImplementedException();
    }
    
}