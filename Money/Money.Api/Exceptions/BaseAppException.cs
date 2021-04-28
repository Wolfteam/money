using Money.Api.Enums;
using System;

namespace Money.Api.Exceptions
{
    public abstract class BaseAppException : Exception
    {
        public AppErrorType ErrorType { get; }

        protected BaseAppException(string message, AppErrorType errorType)
            : base(message)
        {
            ErrorType = errorType;
        }

        private BaseAppException()
            : base()
        {
            ErrorType = AppErrorType.AppInvalidRequest;
        }

        private BaseAppException(string message)
            : base(message)
        {
            ErrorType = AppErrorType.AppInvalidRequest;
        }

        private BaseAppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
