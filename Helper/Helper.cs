using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DatingWeb.Helper
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; public ExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            string errorMessage = string.Empty;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    errorMessage = ex.Message;
                }
                else
                {
                    errorMessage = "Hata var! Kod: " + ex.Message;
                }
                context.Items.Add("exception", ex);
                context.Items.Add("exceptionMessage", errorMessage);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}