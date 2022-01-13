namespace DatingWeb.Model.Request
{
    public class ConfirmPhoneRequest
    {
        public string PhoneNumber { get; set; }
        public string ConfirmCode { get; set; }
    }
}
