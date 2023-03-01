using System.Threading.Tasks;

namespace Platform.Bus.Publisher
{
    public interface IBusPublisher
    {
        ValueTask Publish(byte[] payload, Exchange exchange);
    }
}