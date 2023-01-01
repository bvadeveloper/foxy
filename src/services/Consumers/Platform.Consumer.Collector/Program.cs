using Platform.Host;

namespace Platform.Consumer.Collector
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var types = new[]
            {
                typeof(Platform.Bus.Publisher.Startup),
                typeof(Platform.Bus.Subscriber.Startup),
                typeof(Startup),
            };

            Application.Run(args, types);
        }
    }
}