using Money.Api.Enums;
using System;

namespace Money.Api.Exceptions
{
    public abstract class BaseAppException : Exception
    {
        public AppErrorType ErrorMessageId { get; }

        protected BaseAppException(string message, AppErrorType errorMessageId)
            : base(message)
        {
            ErrorMessageId = errorMessageId;
        }

        private BaseAppException()
            : base()
        {
            ErrorMessageId = AppErrorType.AppInvalidRequest;
        }

        private BaseAppException(string message)
            : base(message)
        {
            ErrorMessageId = AppErrorType.AppInvalidRequest;
        }

        private BaseAppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
