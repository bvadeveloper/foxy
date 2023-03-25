using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus.Abstractions
{
    public interface IBusSubscriber
    {
        Task Subscribe(CancellationToken cancellationToken);

        Task SubscribeByHostIdentifier(string identifier, CancellationToken cancellationToken);

        void Unsubscribe(CancellationToken cancellationToken);
    }
}