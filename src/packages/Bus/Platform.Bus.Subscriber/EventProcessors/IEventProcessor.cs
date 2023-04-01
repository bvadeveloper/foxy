using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber.EventProcessors;

public interface IEventProcessor
{
    Task Process(object sender, BasicDeliverEventArgs arguments);
}