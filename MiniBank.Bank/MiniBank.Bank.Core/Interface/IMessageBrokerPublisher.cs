using MiniBank.Bank.Core.Dto;

namespace MiniBank.Bank.Core.Interface;

public interface IMessageBrokerPublisher
{
    Task PublishAccountCreatedAsync(UserDto user);
}