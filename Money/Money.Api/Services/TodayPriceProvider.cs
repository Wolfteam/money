using Microsoft.Extensions.Logging;
using Money.Api.Dto;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Enums;
using Money.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Api.Services
{
    public class TodayPriceProvider : IPriceProvider
    {
        private readonly ILogger<TodayPriceProvider> _logger;
        private readonly IValidatorService _validatorService;
        private readonly Dictionary<CurrencyType, ITodayPriceProvider> _providers = new Dictionary<CurrencyType, ITodayPriceProvider>();

        public TodayPriceProvider(ILogger<TodayPriceProvider> logger, IValidatorService validatorService)
        {
            _logger = logger;
            _validatorService = validatorService;
        }

        public void AddPriceProvider(ITodayPriceProvider provider)
        {
            _providers.TryAdd(provider.Type, provider);
        }

        public async Task<AppResponseDto<double>> GetTodayPrice(GetTodayCurrencyRequestDto dto)
        {
            if (!_providers.Any())
            {
                throw new InvalidOperationException("No price providers have been registered");
            }

            _logger.LogInformation($"Validating request...");
            _validatorService.ValidateDto(dto);

            _logger.LogInformation($"Trying to retrieve today's price for currency = {dto.Type}...");
            var price = dto.Type switch
            {
                CurrencyType.Dollar => await _providers[dto.Type].GetTodayPrice(),
                CurrencyType.Real => await _providers[dto.Type].GetTodayPrice(),
                _ => throw new NotImplementedException($"The provided currency type = {dto.Type} is currently not supported")
            };

            return new AppResponseDto<double>(price);
        }
    }
}
