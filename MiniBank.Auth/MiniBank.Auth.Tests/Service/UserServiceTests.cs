using Microsoft.Extensions.Logging;
using MiniBank.Auth.Application.Service;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Model;
using Moq;

namespace MiniBank.Auth.Tests.Service;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IMessageBrokerPublisher> _publisherMock = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();

    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(
            _userRepositoryMock.Object,
            _publisherMock.Object,
            _loggerMock.Object
        );
    }
    
    [Fact]
    public async Task GetByIdAsync_UserExists_ReturnsUserDto()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "João",
            Email = "joao@email.com",
            EmailConfirmed = false
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UserDoesNotExist_ThrowsException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(userId));

        Assert.Equal($"User with ID '{userId}' not found.", ex.Message);
    }

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsUserDto()
    {
        var email = "teste@email.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Email = email,
            EmailConfirmed = false
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        var result = await _service.GetByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_UserDoesNotExist_ThrowsException()
    {
        var email = "naoexiste@email.com";

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetByEmailAsync(email));

        Assert.Equal($"User with email '{email}' not found.", ex.Message);
    }

    [Fact]
    public async Task ConfirmEmailAsync_UserExistsAndNotConfirmed_ConfirmsEmail()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Maria",
            Email = "maria@email.com",
            EmailConfirmed = false
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
        _publisherMock.Setup(p => p.PublishEmailConfirmedAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.ConfirmEmailAsync(userId);

        Assert.True(user.EmailConfirmed);
        Assert.Equal(userId, result.Id);

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
        _publisherMock.Verify(p => p.PublishEmailConfirmedAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_UserAlreadyConfirmed_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "confirmado@email.com",
            EmailConfirmed = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConfirmEmailAsync(userId));

        Assert.Equal("Email already confirmed.", ex.Message);
    }

    [Fact]
    public async Task ConfirmEmailAsync_UserDoesNotExist_ThrowsException()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.ConfirmEmailAsync(userId));

        Assert.Equal($"User with ID '{userId}' not found.", ex.Message);
    }
}