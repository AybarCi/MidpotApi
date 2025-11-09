using DatingWeb.Model.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DatingWeb.Helper
{
    public class ResponseWrapper
    {
        private readonly RequestDelegate _next;
        public ResponseWrapper(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var currentBody = context.Response.Body;
            if (context.Request.Path == "/chat/negotiate")
            {
                await this._next(context);
            }
            else if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                // Swagger UI ve JSON endpoint'lerini sarmalama dışında bırak
                await this._next(context);
            }
            //else if (context.Request.Path.StartsWithSegments("/chat", StringComparison.OrdinalIgnoreCase) && context.Request.Query.TryGetValue("access_token", out var accessToken))
            //{
            //    context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");
            //    await _next(context);
            //}
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    context.Response.Body = memoryStream;
                    await _next(context);
                    context.Response.Body = currentBody;
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var readToEnd = new StreamReader(memoryStream).ReadToEnd();
                    object data = JsonConvert.DeserializeObject(readToEnd);
                    string errors = string.Empty;
                    if (context.Items["exception"] != null)
                    {
                        errors = context.Items["exceptionMessage"].ToString();
                    }

                    var result = BaseResponse.Create((HttpStatusCode)context.Response.StatusCode, data, errors);
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
            }
        }
    }
}


