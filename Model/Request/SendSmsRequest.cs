using System;
namespace DatingWeb.Model.Request
{
    public class SendSmsRequest
    {
        public string PhoneNumber { get; set; }
        public string ConfirmCode { get; set; }
    }
}
