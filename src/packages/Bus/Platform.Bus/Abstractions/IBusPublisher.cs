using System.Threading.Tasks;

namespace Platform.Bus.Abstractions
{
    public interface IBusPublisher
    {
        ValueTask Publish(byte[] payload, Exchange exchange);
    }
}