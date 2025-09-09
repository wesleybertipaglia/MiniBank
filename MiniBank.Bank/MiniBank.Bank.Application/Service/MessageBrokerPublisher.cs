using Microsoft.Extensions.Logging;
using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Helper;
using MiniBank.Bank.Core.Interface;
using Newtonsoft.Json;

namespace MiniBank.Bank.Application.Service;

public class MessageBrokerPublisher : IMessageBrokerPublisher
{
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<MessageBrokerPublisher> _logger;

    public MessageBrokerPublisher(
        IMessageBroker messageBroker,
        ILogger<MessageBrokerPublisher> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task PublishAccountCreatedAsync(UserDto user)
    {
        var message = new UserDto
        (
            user.Id,
            user.Name,
            user.Email,
            user.EmailConfirmed
        );

        string jsonMessage;

        try
        {
            jsonMessage = JsonConvert.SerializeObject(message);
            LogHelper.LogInfo(_logger, $"Publishing message to queue 'queue_account_created' for user {user.Id} - {user.Email}");
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, ex, "Failed to serialize message for queue 'queue_account_created'.");
            return;
        }

        try
        {
            await _messageBroker.SendMessageAsync("queue_account_created", jsonMessage);
            LogHelper.LogInfo(_logger, "Message successfully published to queue 'queue_account_created'.");
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, ex, "Failed to publish message to queue 'queue_account_created'.");
        }
    }
}