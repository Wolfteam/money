using Money.Api.Enums;
using System.Threading.Tasks;

namespace Money.Api.Interfaces
{
    public interface ITodayPriceProvider
    {
        CurrencyType Type { get; }
        Task<double> GetTodayPrice();
    }
}
