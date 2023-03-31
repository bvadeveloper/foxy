using System.Threading;

namespace Platform.Bus
{
    public interface IBusSubscriber
    {
        void Subscribe(CancellationToken cancellationToken);

        void Unsubscribe(CancellationToken cancellationToken);
    }
}