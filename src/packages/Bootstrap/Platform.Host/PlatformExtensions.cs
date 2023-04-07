using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host
{
    public static class PlatformExtensions
    {
        /// <summary>
        /// Increase minimum number of worker threads in ThreadPool to overcome performance issue in Redis client and possibly other areas
        /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool.setminthreads?view=netcore-3.1 (use carefully, see caution)
        /// https://stackexchange.github.io/StackExchange.Redis/Timeouts
        /// </summary>
        /// <param name="services"></param>
        /// <param name="multiplicationFactor"></param>
        /// <returns></returns>
        public static IServiceCollection BoostMinWorkerThreads(this IServiceCollection services, byte multiplicationFactor = 10)
        {
            if (multiplicationFactor == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplicationFactor),
                    "It is very likely wrong to set minimum number of threads for thread pool to 0.");
            }

            int originalMinWorkerThreads;
            int originalMinIocThreads;
            ThreadPool.GetMinThreads(out originalMinWorkerThreads, out originalMinIocThreads);

            var newMinWorkerThreads = Environment.ProcessorCount * multiplicationFactor;

            if (ThreadPool.SetMinThreads(newMinWorkerThreads, originalMinIocThreads))
            {
                Console.WriteLine(
                    $"The minimum number of threads for thread pool was set successfully from '{originalMinWorkerThreads}' to '{newMinWorkerThreads}'.");
            }
            else
            {
                Console.WriteLine("The change of minimum number of threads for thread pool was NOT successful.");
            }

            return services;
        }
    }
}