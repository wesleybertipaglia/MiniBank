namespace MiniBank.Bank.Core.Interface;

public interface IMessageBrokerConsumer
{
    Task StartEmailConfirmedConsumer();
}