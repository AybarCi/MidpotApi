using System;

namespace DatingWeb.Model.Request
{
    public class RegisterPremiumUserRequest
    {
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public string ProductId { get; set; }
        public string TransactionId { get; set; }
    }
}
