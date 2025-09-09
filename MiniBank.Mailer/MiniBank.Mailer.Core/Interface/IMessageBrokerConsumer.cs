namespace MiniBank.Mailer.Core.Interface;

public interface IMessageBrokerConsumer
{
    Task StartUserCreatedConsumer();
    Task StartAccountCreatedConsumer();
}