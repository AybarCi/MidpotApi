using System;
using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Data.DbModel
{
    public class CreditProduct
    {
        [Key]
        public string Id { get; set; } // SKU, e.g. credits_5

        public int Credits { get; set; }
        public int PriceCents { get; set; }
    }
}
