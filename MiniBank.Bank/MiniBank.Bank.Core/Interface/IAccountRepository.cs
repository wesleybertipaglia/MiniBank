using MiniBank.Bank.Core.Model;

namespace MiniBank.Bank.Core.Interface;

public interface IAccountRepository
{
    Task<Account> GetByIdAsync(Guid id);
    Task<Account> GetByUserIdAsync(Guid userId);
    Task<Account> CreateAsync(Account account);
    Task<Account> UpdateAsync(Account account);
    Task DeleteAsync(Account account);
}