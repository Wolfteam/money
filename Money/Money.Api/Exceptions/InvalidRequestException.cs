using Money.Api.Enums;
using Money.Api.Exceptions;

namespace Money.Api.Exceptions
{
    public class InvalidRequestException : BaseAppException
    {
        public InvalidRequestException(string message, AppErrorType errorMessageId = AppErrorType.AppInvalidRequest)
            : base(message, errorMessageId)
        {
        }
    }
}
