namespace DatingWeb.Model.Request
{
    public class ForgotPasswordConfirmRequest
    {
        public string PhoneNumber { get; set; }
        public string ConfirmCode { get; set; }
        public string Password { get; set; }
    }
}
