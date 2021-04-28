using Money.Api.Enums;
using Money.Api.Exceptions;

namespace Money.Api.Exceptions
{
    public class NotFoundException : BaseAppException
    {
        public NotFoundException(string message, AppErrorType errorMessageId = AppErrorType.AppNotFound)
            : base(message, errorMessageId)
        {
        }
    }
}
