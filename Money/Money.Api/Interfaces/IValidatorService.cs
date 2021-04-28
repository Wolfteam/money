using System.Runtime.CompilerServices;

namespace Money.Api.Interfaces
{
    public interface IValidatorService
    {
        void ValidateDto<TRequest>(TRequest dto, [CallerMemberName] string callerMember = "");
    }
}
