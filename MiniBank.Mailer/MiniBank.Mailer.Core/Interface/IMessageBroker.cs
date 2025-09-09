namespace MiniBank.Mailer.Core.Interface;

public interface IMessageBroker
{
    Task SendMessageAsync(string queueName, string message);
    Task ReceiveMessageAsync(string queueName, Func<string, Task> onMessageReceived);
}