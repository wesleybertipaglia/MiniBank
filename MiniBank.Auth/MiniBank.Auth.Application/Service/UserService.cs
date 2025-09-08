using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;

namespace MiniBank.Auth.Application.Service;

public class UserService(IUserRepository userRepository, IMessageBrokerPublisher messageBrokerPublisher) : IUserService
{
    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id) 
                   ?? throw new Exception($"User with ID '{id}' not found.");

        return UserMapper.Map(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email) 
                   ?? throw new Exception($"User with email '{email}' not found.");

        return UserMapper.Map(user);
    }

    public async Task<UserDto> ConfirmEmailAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId) 
                   ?? throw new Exception($"User with ID '{userId}' not found.");

        if (user.EmailConfirmed)
            throw new InvalidOperationException("Email already confirmed.");

        user.EmailConfirmed = true;
        await userRepository.UpdateAsync(user);
        await messageBrokerPublisher.PublishEmailConfirmedAsync(user);

        return UserMapper.Map(user);
    }
}
