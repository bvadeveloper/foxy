using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Platform.Bus.Subscriber.Abstractions;

namespace Platform.Bus.Subscriber
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly IBus _bus;
        private readonly IAutoSubscriberMessageDispatcher _messageDispatcher;

        public BusSubscriber(IBus bus, IAutoSubscriberMessageDispatcher messageDispatcher)
        {
            _bus = bus;
            _messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Subscribe to consumers from assembly
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task Subscribe(CancellationToken cancellationToken)
        {
            var subscriber = new AutoSubscriber(_bus, "_")
            {
                ConfigureSubscriptionConfiguration = s => s.WithAutoDelete(),
                GenerateSubscriptionId = info => "_",
                AutoSubscriberMessageDispatcher = _messageDispatcher
            };

            await subscriber.SubscribeAsync(new[] { Assembly.GetEntryAssembly() }, cancellationToken);
        }

        public void Unsubscribe() => _bus.Dispose();
    }
}