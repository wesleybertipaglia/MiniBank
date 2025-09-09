using MiniBank.Mailer.Core.Interface;

namespace MiniBank.Mailer.Application.Service;

public class MessageBrokerPublisher(IMessageBroker messageBroker) : IMessageBrokerPublisher
{
}