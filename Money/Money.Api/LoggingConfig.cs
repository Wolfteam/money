using Money.Api.Controllers;
using Money.Api.Extensions;
using Money.Api.Middleware;
using Money.Api.Models.Logging;
using Money.Api.Services;
using System.Collections.Generic;

namespace Money.Api
{
    public static class LoggingConfig
    {
        public static void SetupLogging()
        {
            var logs = new List<FileToLog>
            {
                //Controller
                new FileToLog(typeof(CurrencyController), "controllers_currency"),
                //Services
                new FileToLog(typeof(DollarService), "services_dollar"),
                new FileToLog(typeof(RealService), "services_real"),
                new FileToLog(typeof(TodayPriceProvider), "services_today_price_provider"),
                new FileToLog(typeof(ValidatorService), "services_validator"),
                //Others
                new FileToLog(typeof(ExceptionHandlerMiddleware), "middleware_exceptions"),
            };

            logs.SetupLogging();
        }
    }
}
