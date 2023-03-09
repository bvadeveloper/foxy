using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus
{
    public interface IBusSubscriber
    {
        ImmutableList<Exchange> ExchangeBindings { get; }

        Task Subscribe(CancellationToken cancellationToken);

        Task SubscribeByLocation(string location, CancellationToken cancellationToken);

        void Unsubscribe(CancellationToken cancellationToken);
    }
}