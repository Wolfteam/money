using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Money.Api.Dto;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Dto.Transactions.Requests;
using Money.Api.Enums;
using Money.Api.Exceptions;
using Money.Api.Interfaces;
using Money.Api.Models;
using Money.Api.Models.Entities;
using Money.Api.Models.Settings;
using Money.Api.Services;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Money.Api.UnitTests.Services
{
    public class TransactionServiceTests
    {
        [Theory]
        [InlineData(CurrencyType.CanadianDollar)]
        [InlineData(CurrencyType.Dollar)]
        [InlineData(CurrencyType.Real)]
        public async Task CanUserMakeAPurchase_UserIsAllowedToMakeAPurchase(CurrencyType currencyType)
        {
            //Arrange
            var userId = 1;
            var fixture = new Fixture();
            var user = fixture.Build<User>().With(x => x.Id, userId).Create();
            var transactions = fixture
                .Build<Transaction>()
                .With(t => t.PurchasedAmount, 200)
                .With(t => t.UserId, userId)
                .With(t => t.CurrencyType, CurrencyType.Dollar)
                .With(t => t.CreatedAt, DateTime.Now.AddMonths(-1))
                .CreateMany();
            var contextMock = new Mock<AppDbContext>();
            contextMock.Setup(s => s.Transactions).ReturnsDbSet(transactions);
            contextMock.Setup(s => s.Users).ReturnsDbSet(new List<User> { user });
            var settings = new TransactionSettings
            {
                MaxPurchasedAmountPerMonthForDollars = 100,
                MaxPurchasedAmountPerMonthForReal = 100
            };

            //Act
            var service = GetTransactionService(contextMock.Object, settings);
            bool canMakeAPurchase = await service.CanUserMakeAPurchase(userId, currencyType, 50);

            //Assert
            switch (currencyType)
            {
                case CurrencyType.Dollar:
                case CurrencyType.Real:
                    canMakeAPurchase.ShouldBeTrue();
                    break;
                default:
                    canMakeAPurchase.ShouldBeFalse();
                    break;
            }
        }

        [Fact]
        public async Task CanUserMakeAPurchase_UserCannotMakeAPurchase()
        {
            //Arrange
            var userId = 1;
            var fixture = new Fixture();
            var transactions = fixture
                .Build<Transaction>()
                .With(t => t.PurchasedAmount, 200)
                .With(t => t.UserId, userId)
                .With(t => t.CurrencyType, CurrencyType.Dollar)
                .With(t => t.CreatedAt, DateTime.Now.AddDays(-1))
                .CreateMany();
            var user = fixture.Build<User>().With(x => x.Id, userId).Create();
            var contextMock = new Mock<AppDbContext>();
            contextMock.Setup(s => s.Transactions).ReturnsDbSet(transactions);
            contextMock.Setup(s => s.Users).ReturnsDbSet(new List<User> { user });
            var settings = new TransactionSettings
            {
                MaxPurchasedAmountPerMonthForDollars = 100,
                MaxPurchasedAmountPerMonthForReal = 100
            };

            //Act
            var service = GetTransactionService(contextMock.Object, settings);
            bool canMakeAPurchase = await service.CanUserMakeAPurchase(userId, CurrencyType.Dollar, 100);

            //Assert
            canMakeAPurchase.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateTransaction_UserDoesNotExist_ThrowNotFoundException()
        {
            //Arrange
            var contextMock = new Mock<AppDbContext>();
            var nonExistingUser = new Fixture().Build<User>().With(u => u.Id, 50).Create();
            contextMock.Setup(x => x.Users).ReturnsDbSet(new List<User> { nonExistingUser });
            var settings = new TransactionSettings
            {
                MaxPurchasedAmountPerMonthForReal = 200,
                MaxPurchasedAmountPerMonthForDollars = 100
            };
            var transactionService = GetTransactionService(contextMock.Object, settings);

            //Act - Assert
            await transactionService.CreateTransaction(new CreateTransactionRequestDto
            {
                CurrencyType = CurrencyType.Dollar,
                AmountToPay = 200,
                UserId = 1
            }).ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateTransaction_UserCannotMakeAPurchase_ThrowsInvalidRequestException()
        {
            //Arrange
            var userId = 1;
            var fixture = new Fixture();
            var transactions = fixture
                .Build<Transaction>()
                .With(t => t.PurchasedAmount, 200)
                .With(t => t.UserId, userId)
                .With(t => t.CurrencyType, CurrencyType.Dollar)
                .With(t => t.CreatedAt, DateTime.Now.AddDays(-1))
                .CreateMany();
            var user = fixture.Build<User>().With(x => x.Id, userId).Create();
            var contextMock = new Mock<AppDbContext>();
            contextMock.Setup(s => s.Transactions).ReturnsDbSet(transactions);
            contextMock.Setup(s => s.Users).ReturnsDbSet(new List<User> { user });
            var settings = new TransactionSettings
            {
                MaxPurchasedAmountPerMonthForDollars = 100,
                MaxPurchasedAmountPerMonthForReal = 100
            };

            //Act - Assert
            var service = GetTransactionService(contextMock.Object, settings);
            await service.CreateTransaction(new CreateTransactionRequestDto
            {
                CurrencyType = CurrencyType.Dollar,
                AmountToPay = 100,
                UserId = userId
            }).ShouldThrowAsync<InvalidRequestException>();
        }

        [Theory]
        [InlineData(CurrencyType.Dollar)]
        [InlineData(CurrencyType.Real)]
        public async Task CreateTransaction_UserCanMakeAPurchase_ReturnsValidResponseDto(CurrencyType currencyType)
        {
            //Arrange
            var userId = 1;
            var paidAmount = 100;
            var todayPrice = 50;
            var fixture = new Fixture();
            var user = fixture.Build<User>().With(x => x.Id, userId).Create();
            var transactions = fixture
                .Build<Transaction>()
                .With(t => t.PurchasedAmount, 200)
                .With(t => t.UserId, userId)
                .With(t => t.CurrencyType, currencyType)
                .With(t => t.CreatedAt, DateTime.Now.AddMonths(-1))
                .CreateMany();
            var contextMock = new Mock<AppDbContext>();
            contextMock.Setup(s => s.Transactions).ReturnsDbSet(transactions);
            contextMock.Setup(s => s.Users).ReturnsDbSet(new List<User> { user });
            var settings = new TransactionSettings
            {
                MaxPurchasedAmountPerMonthForDollars = 100,
                MaxPurchasedAmountPerMonthForReal = 100
            };

            //Act
            var service = GetTransactionService(contextMock.Object, settings, todayPrice);
            var response = await service.CreateTransaction(new CreateTransactionRequestDto
            {
                CurrencyType = currencyType,
                AmountToPay = paidAmount,
                UserId = userId
            });

            //Assert
            response.Succeed.ShouldBeTrue();
            response.Result.UserId.ShouldBe(userId);
            response.Result.PaidAmount.ShouldBe(paidAmount);
            response.Result.PurchasedAmount.ShouldBe(paidAmount / todayPrice);
        }

        private TransactionService GetTransactionService(AppDbContext context, TransactionSettings settings, double todayPrice = 23.21)
        {
            var logger = Mock.Of<ILogger<TransactionService>>();
            var validatorService = Mock.Of<IValidatorService>();
            var priceProviderMock = new Mock<IPriceProvider>();
            priceProviderMock.Setup(s => s.GetTodayPrice(It.IsAny<GetTodayCurrencyRequestDto>())).Returns(Task.FromResult(new AppResponseDto<double>(todayPrice)));
            var options = new Mock<IOptions<TransactionSettings>>();
            options.Setup(s => s.Value).Returns(settings);

            return new TransactionService(logger, validatorService, context, priceProviderMock.Object, options.Object);
        }
    }
}
