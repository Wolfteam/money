using Microsoft.Extensions.Logging;
using Money.Api.Exceptions;
using Money.Api.Interfaces.Api;
using Money.Api.Services;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Money.Api.UnitTests.Services
{
    public class DollarServiceTests
    {
        [Fact]
        public async Task GetTodayPrice_ReturnedPriceIsAValidOne()
        {
            //Arrange
            var expected = "92.3";
            var apiMock = new Mock<IBancoProvinciaApi>();
            apiMock.Setup(s => s.GetTodayDollarPrice()).Returns(() => Task.FromResult(new List<string>
            {
                expected, "98.5", "something goes here"
            }));

            //Act
            var service = GetDollarService(apiMock.Object);
            var todayPrice = await service.GetTodayPrice();

            //Assert
            todayPrice.ToString(CultureInfo.InvariantCulture).ShouldBe(expected);
        }

        [Fact]
        public async Task GetTodayPrice_ReturnedPriceIsNotAValidOne()
        {
            //Arrange
            var expected = "92.3";
            var apiMock = new Mock<IBancoProvinciaApi>();
            apiMock.Setup(s => s.GetTodayDollarPrice()).Returns(() => Task.FromResult(new List<string>
            {
                "98.5", expected, "something goes here"
            }));

            //Act
            var service = GetDollarService(apiMock.Object);
            var todayPrice = await service.GetTodayPrice();

            //Assert
            todayPrice.ToString(CultureInfo.InvariantCulture).ShouldNotBe(expected);
        }

        [Fact]
        public async Task GetTodayPrice_ApiIsDown_ThrowsAnException()
        {
            //Arrange
            var apiMock = new Mock<IBancoProvinciaApi>();
            apiMock.Setup(s => s.GetTodayDollarPrice()).Returns(() => Task.FromResult(new List<string>()));

            //Act
            var service = GetDollarService(apiMock.Object);

            //Assert
            await service.GetTodayPrice().ShouldThrowAsync(typeof(CurrencyException));
        }

        private DollarService GetDollarService(IBancoProvinciaApi api)
        {
            var logger = Mock.Of<ILogger<DollarService>>();
            return new DollarService(logger, api);
        }
    }
}
