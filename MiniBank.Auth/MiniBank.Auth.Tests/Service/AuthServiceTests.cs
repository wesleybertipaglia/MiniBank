using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MiniBank.Auth.Application.Service;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Model;

namespace MiniBank.Auth.Tests.Service;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IMessageBrokerPublisher> _publisherMock = new();
    private readonly Mock<ILogger<AuthService>> _loggerMock = new();

    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _configurationMock.Setup(c => c["Jwt:SecretKey"]).Returns("d9004e5c-475a-461f-ba1f-31360496aa5b");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _service = new AuthService(
            _userRepositoryMock.Object,
            _configurationMock.Object,
            _publisherMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task SignIn_ValidCredentials_ReturnsAuthResponse()
    {
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Maria",
            Email = "maria@email.com",
            Password = hashedPassword
        };

        var request = new SignInRequestDto
        (
            user.Email,
            password
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _service.SignIn(request);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.User.Email);
        Assert.NotEmpty(result.Token.Content);
    }

    [Fact]
    public async Task SignIn_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "joao@email.com",
            Password = BCrypt.Net.BCrypt.HashPassword("correctPassword")
        };

        var request = new SignInRequestDto
        (
            user.Email,
            "wrongPassword"
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SignIn(request));
        Assert.Equal("Invalid email or password", ex.Message);
    }

    [Fact]
    public async Task SignIn_EmailNotFound_ThrowsUnauthorizedAccessException()
    {
        var request = new SignInRequestDto
        (
            "inexistente@email.com",
            "anyPassword"
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User)null!);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SignIn(request));
        Assert.Equal("Invalid email or password", ex.Message);
    }
    
    [Fact]
    public async Task SignUp_EmailNotInUse_CreatesUserAndReturnsAuthResponse()
    {
        var request = new SignUpRequestDto
        (
            "Ana",
            "ana@email.com",
            "123456"
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User)null!);
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                           .ReturnsAsync((User u) => u);

        _publisherMock.Setup(p => p.PublishUserCreatedAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.SignUp(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.User.Email);
        Assert.NotEmpty(result.Token.Content);

        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        _publisherMock.Verify(p => p.PublishUserCreatedAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task SignUp_EmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "jaexiste@email.com",
            Password = "senha"
        };

        var request = new SignUpRequestDto
        (
            existingUser.Email,
            "123",
            "Fulano"
        );

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SignUp(request));
        Assert.Equal("Email is already in use", ex.Message);
    }
}