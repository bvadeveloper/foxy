using System.Net.Http;
using System.Net.Http.Headers;

namespace Platform.Host.Endpoint.Extensions
{
    public static class HeaderExtensions
    {
        public static void AddDefaultHeaders(this HttpRequestMessage request) => 
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    }
}
