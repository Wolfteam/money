using Money.Api.Dto;
using Money.Api.Dto.Transactions.Requests;
using Money.Api.Dto.Transactions.Responses;
using System.Threading.Tasks;

namespace Money.Api.Interfaces
{
    public interface ITransactionService
    {
        Task<AppResponseDto<TransactionResponseDto>> CreateTransaction(CreateTransactionRequestDto dto);
    }
}
