using Money.Api.Enums;

namespace Money.Api.Dto.Currency.Requests
{
    public class GetTodayCurrencyRequestDto
    {
        public CurrencyType Type { get; set; }
    }
}
