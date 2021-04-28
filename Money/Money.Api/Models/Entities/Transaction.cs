using Money.Api.Enums;

namespace Money.Api.Models.Entities
{
    public class Transaction : BaseEntity
    {
        public CurrencyType CurrencyType { get; set; }
        public double PaidAmount { get; set; }
        public double PurchasedAmount { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}
