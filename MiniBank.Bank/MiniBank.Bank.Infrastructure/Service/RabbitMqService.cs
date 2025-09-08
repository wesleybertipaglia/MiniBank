using System.Text;
using Microsoft.Extensions.Logging;
using MiniBank.Bank.Core.Helper;
using MiniBank.Bank.Core.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniBank.Bank.Infrastructure.Service;

public class RabbitMqService : IMessageBroker
{
    private const string _hostName = "localhost";
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(ILogger<RabbitMqService> logger)
    {
        _logger = logger;
    }
    
    public async Task SendMessageAsync(string queueName, string message)
    {
        var factory = new ConnectionFactory() { HostName = _hostName };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(
            queue: queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body
        );

        LogHelper.LogInfo(_logger, $"Sent {message}");
    }

    public async Task ReceiveMessageAsync(string queueName, Func<string, Task> onMessageReceived)
    {
        if (_connection == null)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            _connection = await factory.CreateConnectionAsync();
        }

        _channel ??= await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await onMessageReceived(message);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "Error processing message");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        LogHelper.LogInfo(_logger, $"Listening to queue: {queueName}");
    }
}


