using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Platform.Host.Versioning
{
    public class VersioningMiddleware : IMiddleware
    {
        private string? _version;
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/version"))
            {
                await context.Response.WriteAsync($"version : {GetVersion()}");
            }
            else
            {
                await next(context);
            }
        }

        public string GetVersion()
        {
            if (_version != null)
            {
                return _version;
            }

            var assembly = Assembly.GetEntryAssembly(); 
            var attr = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attr?.InformationalVersion ?? "Undefined";
            _version = version;
            return version;
        }
    }
}
