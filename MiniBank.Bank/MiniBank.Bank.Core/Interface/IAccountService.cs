using MiniBank.Bank.Core.Dto;

namespace MiniBank.Bank.Core.Interface;

public interface IAccountService
{
    Task<AccountResponseDto> OpenAccountByUserIdAsync(UserDto user);
    Task<AccountResponseDto> GetAccountByUserIdAsync(Guid userId);
    Task<AccountResponseDto> DepositByUserIdAsync(Guid userId, DepositRequestDto depositRequestDto);
    Task<AccountResponseDto> WithdrawByUserIdAsync(Guid userId, WithdrawRequestDto withdrawRequestDto);
}