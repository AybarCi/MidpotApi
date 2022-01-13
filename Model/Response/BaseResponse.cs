using System.Linq;
using System.Net;

namespace DatingWeb.Model.Response
{
    public class BaseResponse
    {
        public static BaseResponse Create(HttpStatusCode statusCode, object data = null, string errors = null)
        {
            return new BaseResponse(statusCode, data, errors);
        }
        public bool success => !errors.Any();
        public string errors { get; set; }
        public object data { get; set; }
        protected BaseResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null)
        {
            data = result;
            errors = errorMessage;
        }
    }
}
