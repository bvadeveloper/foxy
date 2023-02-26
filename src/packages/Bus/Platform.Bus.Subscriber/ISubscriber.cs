using System.Threading;
using System.Threading.Tasks;

namespace Platform.Bus.Subscriber
{
    public interface ISubscriber
    {
        Task Subscribe(CancellationToken cancellationToken);
        
        void Unsubscribe(CancellationToken cancellationToken);
    }
}