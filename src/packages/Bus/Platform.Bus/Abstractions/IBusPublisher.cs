using System.Threading.Tasks;

namespace Platform.Bus.Abstractions
{
    public interface IBusPublisher
    {
        ValueTask Publish(byte[] payload, Exchange exchange);

        public ValueTask Publish(byte[] payload, Exchange exchange, byte[] publicKey);
    }
}