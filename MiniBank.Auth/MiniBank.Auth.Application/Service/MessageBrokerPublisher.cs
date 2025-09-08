using Microsoft.Extensions.Logging;
using MiniBank.Auth.Core.Helpers;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;
using MiniBank.Auth.Core.Model;
using Newtonsoft.Json;

namespace MiniBank.Auth.Application.Service;

public class MessageBrokerPublisher : IMessageBrokerPublisher
{
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<MessageBrokerPublisher> _logger;

    public MessageBrokerPublisher(IMessageBroker messageBroker, ILogger<MessageBrokerPublisher> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task PublishUserCreatedAsync(User user)
    {
        var message = UserMapper.Map(user);

        try
        {
            LogHelper.LogInfo(_logger, "Starting to publish message to the 'queue_user_created' queue.");

            var jsonMessage = JsonConvert.SerializeObject(message);
            await _messageBroker.SendMessageAsync("queue_user_created", jsonMessage);

            LogHelper.LogInfo(_logger, "Message successfully published to the 'queue_user_created' queue.");
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, ex, "Error while publishing message to the 'queue_user_created' queue.");
        }
    }

    public async Task PublishEmailConfirmedAsync(User user)
    {
        var message = UserMapper.Map(user);

        try
        {
            LogHelper.LogInfo(_logger, "Starting to publish message to the 'queue_email_confirmed' queue.");

            var jsonMessage = JsonConvert.SerializeObject(message);
            await _messageBroker.SendMessageAsync("queue_email_confirmed", jsonMessage);

            LogHelper.LogInfo(_logger, "Message successfully published to the 'queue_email_confirmed' queue.");
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, ex, "Error while publishing message to the 'queue_email_confirmed' queue.");
        }
    }
}