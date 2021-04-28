using Money.Api.Enums;

namespace Money.Api.Exceptions
{
    public class CurrencyException : BaseAppException
    {
        public CurrencyException(string message, AppErrorType errorMessageId = AppErrorType.AppTodaysPriceCouldNotBeRetrieved)
            : base(message, errorMessageId)
        {
        }
    }
}
