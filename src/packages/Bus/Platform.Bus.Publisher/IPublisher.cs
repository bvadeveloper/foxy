using System.Threading.Tasks;

namespace Platform.Bus.Publisher
{
    public interface IPublisher
    {
        ValueTask Publish(byte[] payload, Exchange exchange);
    }
}