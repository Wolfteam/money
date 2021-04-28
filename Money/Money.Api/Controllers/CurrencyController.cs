using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Dto.Transactions.Requests;
using Money.Api.Interfaces;
using System.Threading.Tasks;

namespace Money.Api.Controllers
{
    public class CurrencyController : BaseController<CurrencyController>
    {
        private readonly IPriceProvider _priceProvider;
        private readonly ITransactionService _transactionService;

        public CurrencyController(
            ILogger<CurrencyController> logger,
            IPriceProvider priceProvider,
            ITransactionService transactionService)
            : base(logger)
        {
            _priceProvider = priceProvider;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodayPrice([FromQuery] GetTodayCurrencyRequestDto dto)
        {
            Logger.LogInformation($"Getting today's price for currency = {dto.Type}...");
            var response = await _priceProvider.GetTodayPrice(dto);

            Logger.LogInformation($"The price for today's currency is = {response.Result}");
            return Ok(response);
        }

        [HttpPost("Purchase")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequestDto dto)
        {
            Logger.LogInformation($"Trying to create a new transaction for userId = {dto.UserId}...");
            var response = await _transactionService.CreateTransaction(dto);

            Logger.LogInformation($"UserId = {dto.UserId} puchased = {response.Result.PurchasedAmount} ({dto.CurrencyType})");
            return Ok(response);
        }
    }
}
