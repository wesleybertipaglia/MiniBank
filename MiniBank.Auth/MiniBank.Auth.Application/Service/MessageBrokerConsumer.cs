using MiniBank.Auth.Core.Interface;

namespace MiniBank.Auth.Application.Service;

public class MessageBrokerConsumer(IMessageBroker messageBroker) : IMessageBrokerConsumer
{
}