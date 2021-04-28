using Money.Api.Enums;
using System;
using System.Text.RegularExpressions;

namespace Money.Api.Extensions
{
    public static class AppErrorTypeExtensions
    {
        public static string GetErrorMsg(this AppErrorType msg)
        {
            return msg switch
            {
                AppErrorType.AppUnknownErrorOccurred => "Unknown error occurred",
                AppErrorType.AppInvalidRequest => "Invalid request",
                AppErrorType.AppTodaysPriceCouldNotBeRetrieved => "Today's price could not be retrieved",
                AppErrorType.AppNotFound => "The resource was not found",
                AppErrorType.AppUserCannotMakeAPurchase => "User cannot make purchases",
                _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
            };
        }

        public static string GetErrorCode(this AppErrorType msg)
        {
            string[] split = Regex.Split($"{msg}", "(?<!^)(?=[A-Z])");
            int msgId = (int)msg;
            return $"{split[0].ToUpper()}_{msgId}";
        }
    }
}
