using FluentValidation;
using Money.Api.Dto.Transactions.Requests;
using Money.Api.Extensions;

namespace Money.Api.Validators.Transactions
{
    public class CreateTransactionRequestDtoValidator : AbstractValidator<CreateTransactionRequestDto>
    {
        public CreateTransactionRequestDtoValidator()
        {
            RuleFor(dto => dto.UserId)
                .GreaterThan(0)
                .WithGlobalErrorCode();

            RuleFor(dto => dto.AmountToPay)
                .GreaterThan(0)
                .WithGlobalErrorCode();

            RuleFor(dto => dto.CurrencyType)
                .SupportedCurrencyType();
        }
    }
}
