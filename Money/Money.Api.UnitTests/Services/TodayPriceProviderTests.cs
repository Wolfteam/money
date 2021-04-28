using Microsoft.Extensions.Logging;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Enums;
using Money.Api.Exceptions;
using Money.Api.Interfaces;
using Money.Api.Services;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Money.Api.UnitTests.Services
{
    public class TodayPriceProviderTests
    {
        private readonly List<CurrencyType> _implementedCurrencyTypes = new List<CurrencyType> { CurrencyType.Dollar, CurrencyType.Real };

        [Theory]
        [InlineData(CurrencyType.Real)]
        [InlineData(CurrencyType.CanadianDollar)]
        [InlineData(CurrencyType.Dollar)]
        public async Task GetTodayPrice_PriceIsReturnedForImplementedTypes(CurrencyType currencyType)
        {
            //Arrange
            var expectedPrice = 20.2;
            var validatorService = Mock.Of<IValidatorService>();

            //Act - Assert
            var service = GeTodayPriceProvider(expectedPrice, currencyType, validatorService);
            if (_implementedCurrencyTypes.Contains(currencyType))
            {
                var response = await service.GetTodayPrice(new GetTodayCurrencyRequestDto
                {
                    Type = currencyType
                });

                response.Succeed.ShouldBeTrue();
                response.Result.ShouldBe(expectedPrice);
            }
            else
            {
                await service.GetTodayPrice(new GetTodayCurrencyRequestDto
                {
                    Type = currencyType
                }).ShouldThrowAsync(typeof(NotImplementedException));
            }
        }

        [Theory]
        [InlineData(CurrencyType.Real)]
        [InlineData(CurrencyType.CanadianDollar)]
        [InlineData(CurrencyType.Dollar)]
        public async Task GetTodayPrice_RequestIsNotValid(CurrencyType currencyType)
        {
            //Arrange
            var expectedPrice = 50.21;
            var validatorServiceMock = new Mock<IValidatorService>();
            switch (currencyType)
            {
                case CurrencyType.Dollar:
                case CurrencyType.Real:
                    break;
                default:
                    validatorServiceMock
                        .Setup(s => s.ValidateDto(It.IsAny<object>(), It.IsAny<string>()))
                        .Throws<ValidationException>();
                    break;
            }

            //Act - Arrange
            var service = GeTodayPriceProvider(expectedPrice, currencyType, validatorServiceMock.Object);
            if (_implementedCurrencyTypes.Contains(currencyType))
            {
                var response = await service.GetTodayPrice(new GetTodayCurrencyRequestDto
                {
                    Type = currencyType
                });

                response.Succeed.ShouldBeTrue();
                response.Result.ShouldBe(expectedPrice);
            }
            else
            {
                await service.GetTodayPrice(new GetTodayCurrencyRequestDto
                {
                    Type = currencyType
                }).ShouldThrowAsync(typeof(ValidationException));
            }
        }

        [Theory]
        [InlineData(CurrencyType.Real)]
        [InlineData(CurrencyType.CanadianDollar)]
        [InlineData(CurrencyType.Dollar)]
        public async Task GetTodayPrice_NoPriceProviderHasBeenRegistered_ThrowsAnException(CurrencyType currencyType)
        {
            //Arrange
            var service = GeTodayPriceProvider(2, currencyType, Mock.Of<IValidatorService>(), false);

            //Act - Assert
            await service.GetTodayPrice(new GetTodayCurrencyRequestDto
            {
                Type = currencyType
            }).ShouldThrowAsync<InvalidOperationException>();
        }

        private TodayPriceProvider GeTodayPriceProvider(
            double returnedPrice,
            CurrencyType currencyType,
            IValidatorService validatorService,
            bool registerProvider = true)
        {
            var logger = Mock.Of<ILogger<TodayPriceProvider>>();
            var priceProviderMock = new Mock<ITodayPriceProvider>();
            priceProviderMock.Setup(s => s.Type).Returns(currencyType);
            priceProviderMock.Setup(s => s.GetTodayPrice()).Returns(Task.FromResult(returnedPrice));
            var service = new TodayPriceProvider(logger, validatorService);
            if (registerProvider)
                service.AddPriceProvider(priceProviderMock.Object);
            return service;
        }
    }
}
