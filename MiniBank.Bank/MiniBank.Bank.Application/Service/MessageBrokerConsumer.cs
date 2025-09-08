using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Helper;
using MiniBank.Bank.Core.Interface;
using Newtonsoft.Json;

namespace MiniBank.Bank.Application.Service;

public class MessageBrokerConsumer : IMessageBrokerConsumer
{
    private readonly IMessageBroker _messageBroker;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageBrokerConsumer> _logger;

    public MessageBrokerConsumer(
        IMessageBroker messageBroker,
        IServiceScopeFactory scopeFactory,
        ILogger<MessageBrokerConsumer> logger)
    {
        _messageBroker = messageBroker;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartEmailConfirmedConsumer()
    {
        LogHelper.LogInfo(_logger, "Starting consumer for queue 'queue_email_confirmed'.");

        await _messageBroker.ReceiveMessageAsync("queue_email_confirmed", async message =>
        {
            LogHelper.LogInfo(_logger, $"Message received from queue 'queue_email_confirmed': {message}");

            using var scope = _scopeFactory.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

            UserDto? user;
            try
            {
                user = JsonConvert.DeserializeObject<UserDto>(message);
                if (user is null)
                {
                    LogHelper.LogWarning(_logger, "Invalid message: could not deserialize to UserDto.");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "Failed to deserialize message.");
                return;
            }

            try
            {
                LogHelper.LogInfo(_logger, $"Opening account for user: {user.Id} - {user.Email}");
                await accountService.OpenAccountByUserIdAsync(user);
                LogHelper.LogInfo(_logger, $"Account successfully created for user {user.Id}.");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, $"Failed to process message for user {user.Id}.");
            }
        });
    }
}
