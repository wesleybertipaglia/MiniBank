using Microsoft.Extensions.Logging;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Helper;
using MiniBank.Auth.Core.Helpers;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;

namespace MiniBank.Auth.Application.Service;

public class UserService(
    IUserRepository userRepository, 
    IMessageBrokerPublisher messageBrokerPublisher,
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        LogHelper.LogInfo(logger, "Fetching user by ID: {UserId}", [id]);

        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
        {
            LogHelper.LogWarning(logger, "User with ID {UserId} not found.", [id]);
            throw new Exception($"User with ID '{id}' not found.");
        }

        LogHelper.LogInfo(logger, "User found: {UserId}", [id]);
        return UserMapper.Map(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        LogHelper.LogInfo(logger, "Fetching user by email: {Email}", [email]);

        var user = await userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            LogHelper.LogWarning(logger, "User with email {Email} not found.", [email]);
            throw new Exception($"User with email '{email}' not found.");
        }

        LogHelper.LogInfo(logger, "User found with email: {Email}", [email]);
        return UserMapper.Map(user);
    }

    public async Task<UserDto> ConfirmEmailAsync(Guid userId)
    {
        LogHelper.LogInfo(logger, "Starting email confirmation for user: {UserId}", [userId]);

        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            LogHelper.LogWarning(logger, "User with ID {UserId} not found for email confirmation.", [userId]);
            throw new Exception($"User with ID '{userId}' not found.");
        }

        if (user.EmailConfirmed)
        {
            LogHelper.LogWarning(logger, "Attempt to confirm an already confirmed email for user {UserId}.", [userId]);
            throw new InvalidOperationException("Email already confirmed.");
        }

        user.EmailConfirmed = true;
        await userRepository.UpdateAsync(user);

        LogHelper.LogInfo(logger, "Email confirmed for user {UserId}.", [userId]);

        await messageBrokerPublisher.PublishEmailConfirmedAsync(user);

        LogHelper.LogInfo(logger, "Email confirmation message published for user {UserId}.", [userId]);

        return UserMapper.Map(user);
    }
}