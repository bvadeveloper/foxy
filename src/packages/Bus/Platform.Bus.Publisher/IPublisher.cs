using System.Threading.Tasks;

namespace Platform.Bus.Publisher
{
    public interface IPublisher
    {
        Task Publish(byte[] payload, Exchange exchange);
    }
}