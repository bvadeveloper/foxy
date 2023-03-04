using System.Threading.Tasks;

namespace Platform.Bus
{
    public interface IBusPublisher
    {
        ValueTask Publish(byte[] payload, Exchange exchange);
    }
}