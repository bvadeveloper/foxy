using System.Threading;
using System.Threading.Tasks;

namespace Platform.Host.Endpoint.Abstractions
{
    public interface IClientFactory
    {
        /// <summary>
        /// Define the service endpoint by service name
        /// and make a call
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="relativePath"></param>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> Send<T>(string serviceName, string relativePath, object payload,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Define the service endpoint by service name
        /// and make a call
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="relativePath"></param>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task Send(string serviceName, string relativePath, object payload,
            CancellationToken cancellationToken = default);
    }
}