using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus
{
    public interface IBusSubscriber
    {
        ImmutableList<Exchange> ExchangeBindings { get; }
        
        Task Subscribe(CancellationToken cancellationToken);

        void Unsubscribe(CancellationToken cancellationToken);

        Task SubscribeByGeoLocation(string marker, CancellationToken cancellationToken);
    }
}