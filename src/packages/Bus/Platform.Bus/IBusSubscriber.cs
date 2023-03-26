using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus
{
    public interface IBusSubscriber
    {
        void Subscribe(CancellationToken cancellationToken);

        void SubscribeByHostIdentifier(string identifier, CancellationToken cancellationToken);

        void Unsubscribe(CancellationToken cancellationToken);
    }
}