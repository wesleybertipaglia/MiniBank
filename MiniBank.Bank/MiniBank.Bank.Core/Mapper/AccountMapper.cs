using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Model;

namespace MiniBank.Bank.Core.Mapper;

public static class AccountMapper
{
    public static AccountResponseDto Map(Account account)
    {
        return new AccountResponseDto
        (
            account.Id,
            account.Balance,
            account.CreatedAt,
            account.UpdatedAt
        );
    }
}