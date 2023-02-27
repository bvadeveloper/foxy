using Platform.Host;

namespace Platform.Telegram.Bot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var types = new[]
            {
                typeof(Platform.Bus.Publisher.Startup),
                // typeof(Platform.Bus.Subscriber.Startup),
                typeof(Platform.Caching.Redis.Startup),
                typeof(Platform.Limiter.Redis.Startup),
                typeof(Platform.Validation.Fluent.Startup),
                typeof(Platform.Serializer.MemoryPack.Startup),
                typeof(Startup),
            };

            Application.Run(args, types);
        }
    }
}