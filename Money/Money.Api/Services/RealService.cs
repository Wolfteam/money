using Microsoft.Extensions.Logging;
using Money.Api.Enums;
using Money.Api.Interfaces.Providers;
using System.Threading.Tasks;

namespace Money.Api.Services
{
    public class RealService : ITodayRealPriceProvider
    {
        private readonly ILogger<RealService> _logger;
        private readonly ITodayDollarPriceProvider _todayDollarPriceProvider;

        public CurrencyType Type => CurrencyType.Real;

        public RealService(
            ILogger<RealService> logger,
            ITodayDollarPriceProvider todayDollarPriceProvider)
        {
            _logger = logger;
            _todayDollarPriceProvider = todayDollarPriceProvider;
        }

        public async Task<double> GetTodayPrice()
        {
            _logger.LogInformation("Retrieving today's dollar price...");
            var dollar = await _todayDollarPriceProvider.GetTodayPrice();

            var real = dollar / 4;
            _logger.LogInformation($"Today's dollar price is = {dollar} and the real is = {real}");
            //TODO: CHANGE THIS IMPL ONCE THE EXTERNAL TEAM COMPLETES THEIR API
            return real;
        }
    }
}
