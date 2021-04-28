using Money.Api.Dto;
using Money.Api.Dto.Currency.Requests;
using System.Threading.Tasks;

namespace Money.Api.Interfaces
{
    public interface IPriceProvider
    {
        Task<AppResponseDto<double>> GetTodayPrice(GetTodayCurrencyRequestDto dto);
    }
}
