using Platform.Host;

namespace Platform.Processor.GeoCoordinator
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var types = new[]
            {
                // typeof(Platform.Bus.Publisher.Startup),
                // typeof(Platform.Bus.Subscriber.Startup),
                typeof(Platform.Bus.Rmq.Startup),
                typeof(Startup),
            };

            Application.Run(args, types);
        }
    }
}