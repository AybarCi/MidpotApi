using System;

namespace DatingWeb.Model.Response
{
    public class PremiumUserResponse
    {
        public long UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public string ProductId { get; set; }
        public string TransactionId { get; set; }
    }
}
