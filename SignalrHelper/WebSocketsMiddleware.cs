using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DatingWeb.SignalrHelper
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/chat", StringComparison.OrdinalIgnoreCase) && httpContext.Request.Query.TryGetValue("access_token", out var accessToken))
                httpContext.Request.Headers.Add("Authorization", $"Bearer {accessToken}");

            await _next(httpContext);
        }
    }
}
