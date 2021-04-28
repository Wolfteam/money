using Microsoft.Extensions.Logging;
using Money.Api.Enums;
using Money.Api.Exceptions;
using Money.Api.Interfaces.Api;
using Money.Api.Interfaces.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Api.Services
{
    public class DollarService : ITodayDollarPriceProvider
    {
        private readonly ILogger<DollarService> _logger;
        private readonly IBancoProvinciaApi _api;

        public CurrencyType Type => CurrencyType.Dollar;

        public DollarService(ILogger<DollarService> logger, IBancoProvinciaApi api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task<double> GetTodayPrice()
        {
            _logger.LogInformation("Trying to retrieve today's dollar price...");
            try
            {
                var values = await _api.GetTodayDollarPrice();
                if (!values.Any())
                {
                    throw new CurrencyException("Couldn't retrieve the dollar price");
                }

                if (!double.TryParse(values.First(), out double price))
                {
                    throw new CurrencyException("Couldn't retrieve the dollar price");
                }

                _logger.LogInformation($"Today's dollar price is = {price}");
                return price;
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown error occurred", e);
                throw;
            }
        }
    }
}
