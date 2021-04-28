using Money.Api.Enums;
using System;

namespace Money.Api.Dto.Transactions.Responses
{
    public class TransactionResponseDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public double PaidAmount { get; set; }
        public double PurchasedAmount { get; set; }

        public long UserId { get; set; }
    }
}
