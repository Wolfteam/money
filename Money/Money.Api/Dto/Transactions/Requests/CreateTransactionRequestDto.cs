using Money.Api.Enums;

namespace Money.Api.Dto.Transactions.Requests
{
    public class CreateTransactionRequestDto
    {
        public long UserId { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public double AmountToPay { get; set; }
    }
}
