using Microsoft.Extensions.Logging;
using Moq;
using MiniBank.Bank.Application.Service;
using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Interface;
using MiniBank.Bank.Core.Model;

namespace MiniBank.Bank.Tests.Service;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IMessageBrokerPublisher> _publisherMock = new();
    private readonly Mock<ILogger<AccountService>> _loggerMock = new();

    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _service = new AccountService(
            _accountRepositoryMock.Object,
            _publisherMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task OpenAccountByUserIdAsync_WhenAccountDoesNotExist_CreatesAccount()
    {
        var user = new UserDto(Guid.NewGuid(), "Maria", "maria@email.com", true);

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(user.Id))
                              .ReturnsAsync((Account)null!);

        _accountRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
                              .ReturnsAsync((Account a) => a);

        _publisherMock.Setup(p => p.PublishAccountCreatedAsync(user))
                      .Returns(Task.CompletedTask);

        var result = await _service.OpenAccountByUserIdAsync(user);

        Assert.Equal(user.Id, result.UserId);
        _accountRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        _publisherMock.Verify(p => p.PublishAccountCreatedAsync(user), Times.Once);
    }

    [Fact]
    public async Task OpenAccountByUserIdAsync_WhenAccountExists_ThrowsInvalidOperationException()
    {
        var user = new UserDto(Guid.NewGuid(), "João", "joao@email.com", true);
        var existingAccount = new Account { Id = Guid.NewGuid(), UserId = user.Id, Balance = 100 };

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(user.Id)).ReturnsAsync(existingAccount);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.OpenAccountByUserIdAsync(user));

        Assert.Equal("Account already exists for this user.", ex.Message);
    }

    [Fact]
    public async Task GetAccountByUserIdAsync_WhenAccountExists_ReturnsAccount()
    {
        var userId = Guid.NewGuid();
        var account = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 200 };

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(account);

        var result = await _service.GetAccountByUserIdAsync(userId);

        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetAccountByUserIdAsync_WhenAccountNotFound_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                              .ReturnsAsync((Account)null!);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetAccountByUserIdAsync(userId));

        Assert.Equal("Account not found for this user.", ex.Message);
    }

    [Fact]
    public async Task DepositByUserIdAsync_WhenAccountExists_DepositsAmount()
    {
        var userId = Guid.NewGuid();
        var account = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 100 };
        var deposit = new DepositRequestDto(50);

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(account);
        _accountRepositoryMock.Setup(r => r.UpdateAsync(account)).ReturnsAsync(account);

        var result = await _service.DepositByUserIdAsync(userId, deposit);

        Assert.Equal(150, result.Balance);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
    }

    [Fact]
    public async Task DepositByUserIdAsync_WhenAccountNotFound_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var deposit = new DepositRequestDto(100);

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                              .ReturnsAsync((Account)null!);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.DepositByUserIdAsync(userId, deposit));

        Assert.Equal("Account not found for this user.", ex.Message);
    }

    [Fact]
    public async Task WithdrawByUserIdAsync_WhenAccountExists_WithdrawsAmount()
    {
        var userId = Guid.NewGuid();
        var account = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 200 };
        var withdraw = new WithdrawRequestDto(100);

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(account);
        _accountRepositoryMock.Setup(r => r.UpdateAsync(account)).ReturnsAsync(account);

        var result = await _service.WithdrawByUserIdAsync(userId, withdraw);

        Assert.Equal(100, result.Balance);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
    }

    [Fact]
    public async Task WithdrawByUserIdAsync_WhenAccountNotFound_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var withdraw = new WithdrawRequestDto(100);

        _accountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                              .ReturnsAsync((Account)null!);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.WithdrawByUserIdAsync(userId, withdraw));

        Assert.Equal("Account not found for this user.", ex.Message);
    }
}
