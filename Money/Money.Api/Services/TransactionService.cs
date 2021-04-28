using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Money.Api.Dto;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Dto.Transactions.Requests;
using Money.Api.Dto.Transactions.Responses;
using Money.Api.Enums;
using Money.Api.Exceptions;
using Money.Api.Interfaces;
using Money.Api.Models;
using Money.Api.Models.Entities;
using Money.Api.Models.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private readonly IValidatorService _validatorService;
        private readonly AppDbContext _dbContext;
        private readonly IPriceProvider _priceProvider;
        private readonly TransactionSettings _settings;

        public TransactionService(
            ILogger<TransactionService> logger,
            IValidatorService validatorService,
            AppDbContext dbContext,
            IPriceProvider priceProvider,
            IOptions<TransactionSettings> settings)
        {
            _logger = logger;
            _validatorService = validatorService;
            _dbContext = dbContext;
            _priceProvider = priceProvider;
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings), "You need to provide valid transaction settings");

            if (_settings.MaxPurchasedAmountPerMonthForDollars <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(_settings.MaxPurchasedAmountPerMonthForDollars),
                    _settings.MaxPurchasedAmountPerMonthForDollars,
                    "The provided value is not valid");
            }

            if (_settings.MaxPurchasedAmountPerMonthForReal <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(_settings.MaxPurchasedAmountPerMonthForReal),
                    _settings.MaxPurchasedAmountPerMonthForReal,
                    "The provided value is not valid");
            }
        }

        public async Task<AppResponseDto<TransactionResponseDto>> CreateTransaction(CreateTransactionRequestDto dto)
        {
            _logger.LogInformation("Validating request...");
            _validatorService.ValidateDto(dto);

            if (!_dbContext.Users.Any(u => u.Id == dto.UserId))
            {
                var msg = $"UserId = {dto.UserId} does not exist";
                _logger.LogWarning(msg);
                throw new NotFoundException(msg);
            }

            _logger.LogInformation($"Getting today's price for currency = {dto.CurrencyType}...");
            var todayPrice = await _priceProvider.GetTodayPrice(new GetTodayCurrencyRequestDto
            {
                Type = dto.CurrencyType
            });
            var purchasedAmount = dto.AmountToPay / todayPrice.Result;

            _logger.LogInformation($"Checking if userId = {dto.UserId} can make this purchase...");
            bool canMakeAPurchase = await CanUserMakeAPurchase(dto.UserId, dto.CurrencyType, purchasedAmount);
            if (!canMakeAPurchase)
            {
                var msg = $"UserId = {dto.UserId} is not allowed to make more purchases this month";
                _logger.LogWarning(msg);
                throw new InvalidRequestException(msg, AppErrorType.AppUserCannotMakeAPurchase);
            }

            var transaction = new Transaction
            {
                UserId = dto.UserId,
                PaidAmount = dto.AmountToPay,
                PurchasedAmount = purchasedAmount,
                CreatedAt = DateTime.Now,
                CreatedBy = "system"
            };

            _logger.LogInformation("Saving the changes made in this context...");
            _dbContext.Add(transaction);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"TransactionId = {transaction.Id} was successfully created");
            return new AppResponseDto<TransactionResponseDto>(new TransactionResponseDto
            {
                UserId = transaction.UserId,
                CreatedAt = transaction.CreatedAt,
                CurrencyType = transaction.CurrencyType,
                PaidAmount = transaction.PaidAmount,
                PurchasedAmount = transaction.PurchasedAmount,
                Id = transaction.Id
            });
        }

        public async Task<bool> CanUserMakeAPurchase(long userId, CurrencyType currencyType, double amountToPurchase)
        {
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            var to = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);
            var purchasedAmount = await _dbContext.Transactions
                .Where(u => u.UserId == userId && u.CurrencyType == currencyType && u.CreatedAt >= from && u.CreatedAt <= to)
                .SumAsync(t => t.PurchasedAmount);
            return currencyType switch
            {
                CurrencyType.Dollar => purchasedAmount + amountToPurchase <= _settings.MaxPurchasedAmountPerMonthForDollars,
                CurrencyType.Real => purchasedAmount + amountToPurchase <= _settings.MaxPurchasedAmountPerMonthForReal,
                _ => false
            };
        }
    }
}
