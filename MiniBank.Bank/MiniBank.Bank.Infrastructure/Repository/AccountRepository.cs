using Microsoft.EntityFrameworkCore;
using MiniBank.Bank.Core.Interface;
using MiniBank.Bank.Core.Model;
using MiniBank.Bank.Infrastructure.Data;

namespace MiniBank.Bank.Infrastructure.Repository;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task<Account> GetByIdAsync(Guid id)
    {
        return await context.Accounts.FindAsync(id);
    }

    public async Task<Account> GetByUserIdAsync(Guid userId)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Account> CreateAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(Account account)
    {
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
    }
}
