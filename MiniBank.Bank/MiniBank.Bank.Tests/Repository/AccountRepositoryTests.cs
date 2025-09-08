using Microsoft.EntityFrameworkCore;
using MiniBank.Bank.Core.Model;
using MiniBank.Bank.Infrastructure.Data;
using MiniBank.Bank.Infrastructure.Repository;

namespace MiniBank.Bank.Tests.Repository;

public class AccountRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AccountRepository _repository;

    public AccountRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new AccountRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_AddsAccountToDatabase()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = 1000
        };

        var result = await _repository.CreateAsync(account);

        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
        Assert.Single(_context.Accounts);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAccount_WhenExists()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = 500
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(account.Id);

        Assert.NotNull(result);
        Assert.Equal(account.UserId, result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsAccount_WhenExists()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = 750
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserIdAsync(account.UserId);

        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAccountInDatabase()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = 300
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        account.Balance = 600;
        await _repository.UpdateAsync(account);

        var updated = await _context.Accounts.FindAsync(account.Id);
        Assert.Equal(600, updated!.Balance);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAccountFromDatabase()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Balance = 200
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(account);

        var deleted = await _context.Accounts.FindAsync(account.Id);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
