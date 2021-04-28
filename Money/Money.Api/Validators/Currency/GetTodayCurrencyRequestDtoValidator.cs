using FluentValidation;
using Money.Api.Dto.Currency.Requests;
using Money.Api.Extensions;

namespace Money.Api.Validators.Currency
{
    public class GetTodayCurrencyRequestDtoValidator : AbstractValidator<GetTodayCurrencyRequestDto>
    {
        public GetTodayCurrencyRequestDtoValidator()
        {
            RuleFor(dto => dto.Type)
                .SupportedCurrencyType();
        }
    }
}
