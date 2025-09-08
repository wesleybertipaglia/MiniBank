using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Helpers;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;

namespace MiniBank.Auth.Application.Service;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration,
    IMessageBrokerPublisher messageBrokerPublisher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponseDto> SignIn(SignInRequestDto signInRequestDto)
    {
        LogHelper.LogInfo(logger, "Attempting to sign in user with email: {Email}", [signInRequestDto.Email]);

        var user = await userRepository.GetByEmailAsync(signInRequestDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(signInRequestDto.Password, user.Password))
        {
            LogHelper.LogWarning(logger, "Invalid credentials for email: {Email}", [signInRequestDto.Email]);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = TokenHelper.GenerateJwtToken(configuration, user);

        LogHelper.LogInfo(logger, "User signed in successfully: {UserId}", [user.Id]);

        return AuthMapper.Map
        (
            UserMapper.Map(user),
            TokenMapper.Map(token)
        );
    }

    public async Task<AuthResponseDto> SignUp(SignUpRequestDto signUpRequestDto)
    {
        LogHelper.LogInfo(logger, "Attempting to sign up new user with email: {Email}", [signUpRequestDto.Email]);

        var existingUser = await userRepository.GetByEmailAsync(signUpRequestDto.Email);
        
        if (existingUser != null)
        {
            LogHelper.LogWarning(logger, "Email already in use: {Email}", [signUpRequestDto.Email]);
            throw new InvalidOperationException("Email is already in use");
        }

        var user = UserMapper.Map(signUpRequestDto);
        await userRepository.CreateAsync(user);

        LogHelper.LogInfo(logger, "New user created: {UserId}", [user.Id]);

        var token = TokenHelper.GenerateJwtToken(configuration, user);

        await messageBrokerPublisher.PublishUserCreatedAsync(user);

        LogHelper.LogInfo(logger, "User creation message published for user: {UserId}", [user.Id]);

        return AuthMapper.Map
        (
            UserMapper.Map(user),
            TokenMapper.Map(token)
        );
    }
}