using Microsoft.Extensions.Logging;
using Money.Api.Interfaces.Providers;
using Money.Api.Services;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Money.Api.UnitTests.Services
{
    public class RealServiceTests
    {
        [Fact]
        public async Task GetTodayPrice_ReturnedPriceIsAValidOne()
        {
            //Arrange
            double todayDollarPrice = 92;
            var logger = Mock.Of<ILogger<RealService>>();
            var dollarProvider = new Mock<ITodayDollarPriceProvider>();
            dollarProvider.Setup(s => s.GetTodayPrice()).Returns(() => Task.FromResult(todayDollarPrice));

            //Act
            var service = new RealService(logger, dollarProvider.Object);
            var todayPrice = await service.GetTodayPrice();

            //Assert
            todayPrice.ShouldBe(todayDollarPrice / 4);
        }
    }
}
