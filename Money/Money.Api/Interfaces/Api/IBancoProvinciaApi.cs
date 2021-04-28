using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Money.Api.Interfaces.Api
{
    public interface IBancoProvinciaApi
    {
        [Get("/Principal/Dolar")]
        Task<List<string>> GetTodayDollarPrice();
    }
}
