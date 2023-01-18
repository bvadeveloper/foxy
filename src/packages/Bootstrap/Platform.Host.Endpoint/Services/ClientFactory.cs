using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Platform.Host.Endpoint.Abstractions;
using Platform.Host.Endpoint.Configurations;
using Platform.Host.Endpoint.Extensions;
using Platform.Logging.Extensions;

namespace Platform.Host.Endpoint.Services
{
    public class ClientFactory : IClientFactory
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly List<ServiceEndpointConfiguration> _endpointConfigurations;
        private readonly IResponseConverter _responseConverter;
        private readonly ILogger _logger;

        public ClientFactory(IOptions<List<ServiceEndpointConfiguration>> options,
            IHttpClientFactory clientFactory,
            IResponseConverter responseConverter,
            ILogger<ClientFactory> logger)
        {
            _clientFactory = clientFactory;
            _endpointConfigurations = options.Value;
            _responseConverter = responseConverter;
            _logger = logger;
        }

        public async Task<T> Send<T>(string serviceName, string relativePath, object payload,
            CancellationToken cancellationToken = default)
        {
            using var client = MakeClient(serviceName);
            var httpRequest = MakeRequest(new Uri(client.BaseAddress, relativePath), payload);
            var httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.Warn(
                    $"The request to '{client.BaseAddress}' not succeeded, status code '{httpResponse.StatusCode}'");
                return default!;
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var response = _responseConverter.Deserialize<T>(responseString);

            return response;
        }

        public async Task Send(string serviceName, string relativePath, object payload,
            CancellationToken cancellationToken = default)
        {
            using var client = MakeClient(serviceName);
            var httpRequest = MakeRequest(new Uri(client.BaseAddress, relativePath), payload);
            var httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.Warn(
                    $"The request to '{client.BaseAddress}' not succeeded, status code '{httpResponse.StatusCode}'");

                throw new Exception($"The request to '{client.BaseAddress}' not succeeded, status code '{httpResponse.StatusCode}'");
            }
        }

        private HttpClient MakeClient(string name) =>
            _endpointConfigurations
                .Where(c => c.ServiceName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Select(c =>
                {
                    var client = _clientFactory.CreateClient();
                    client.BaseAddress = new Uri(c.Url);
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", name);

                    return client;
                })
                .First();

        private static HttpRequestMessage MakeRequest(Uri url, object payload)
        {
            var bytePayload = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(payload));

            var content = new ByteArrayContent(bytePayload);
            content.Headers.ContentLength = bytePayload.Length;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = url,
                Content = content,
            };

            request.AddDefaultHeaders();

            return request;
        }
    }
}