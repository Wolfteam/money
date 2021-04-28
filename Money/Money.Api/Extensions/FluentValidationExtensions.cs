using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using Money.Api.Enums;

namespace Money.Api.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            string errorCode)
        {
            return rule.Configure(c =>
            {
                foreach (var ruleComponent in c.Components)
                {
                    var component = (RuleComponent<T, TProperty>)ruleComponent;
                    component.ErrorCode = errorCode;
                }
            });
        }

        public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            AppErrorType error = AppErrorType.AppInvalidRequest)
        {
            return rule.WithGlobalErrorCode(error.GetErrorCode());
        }

        public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorMsgAndCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            AppErrorType error)
        {
            return rule.WithGlobalErrorMsgAndCode(error.GetErrorMsg(), error.GetErrorCode());
        }

        public static IRuleBuilderOptions<T, TProperty> WithGlobalErrorMsgAndCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            string errorMessage,
            string errorCode)
        {
            return rule.Configure(c =>
            {
                foreach (var ruleComponent in c.Components)
                {
                    var component = (RuleComponent<T, TProperty>)ruleComponent;
                    component.SetErrorMessage(errorMessage);
                    component.ErrorCode = errorCode;
                }
            });
        }

        public static IRuleBuilderOptions<T, CurrencyType> SupportedCurrencyType<T>(this IRuleBuilderInitial<T, CurrencyType> rule)
        {
            var notSupportedTypes = new[] { CurrencyType.CanadianDollar };
            return rule
                   .IsInEnum()
                   .Must(val => !notSupportedTypes.Contains(val))
                   .WithGlobalErrorMsgAndCode("The provided currency type is not valid", AppErrorType.AppInvalidRequest.GetErrorCode());
        }
    }
}
