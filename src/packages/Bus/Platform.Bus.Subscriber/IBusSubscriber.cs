using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus.Subscriber
{
    public interface IBusSubscriber
    {
        Task Subscribe(CancellationToken cancellationToken);
        
        void Unsubscribe(CancellationToken cancellationToken);
    }
}