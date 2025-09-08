using MiniBank.Auth.Core.Model;

namespace MiniBank.Auth.Core.Interface;

public interface IMessageBrokerPublisher
{
    Task PublishUserCreatedAsync(User user);
    Task PublishEmailConfirmedAsync(User user);
}