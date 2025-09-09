using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniBank.Mailer.Core.Dto;
using MiniBank.Mailer.Core.Helper;
using MiniBank.Mailer.Core.Interface;
using Newtonsoft.Json;

namespace MiniBank.Mailer.Application.Service;

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

    public async Task StartUserCreatedConsumer()
    {
        LogHelper.LogInfo(_logger, "Starting consumer for queue 'queue_user_created'.");

        await _messageBroker.ReceiveMessageAsync("queue_user_created", async message =>
        {
            LogHelper.LogInfo(_logger, $"Message received from queue 'queue_user_created': {message}");

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

            var (subject, body) = EmailTemplateFactory.BuildConfirmationEmail(user);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                LogHelper.LogInfo(_logger, $"Sending confirmation email to {user.Email}");
                await emailService.SendEmailAsync(
                    to: user.Email,
                    subject: subject,
                    body: body
                );
                LogHelper.LogInfo(_logger, $"Confirmation email successfully sent to {user.Email}.");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, $"Failed to send confirmation email to {user.Email}.");
            }
        });
    }

    public async Task StartAccountCreatedConsumer()
    {
        LogHelper.LogInfo(_logger, "Starting consumer for queue 'queue_account_created'.");

        await _messageBroker.ReceiveMessageAsync("queue_account_created", async message =>
        {
            LogHelper.LogInfo(_logger, $"Message received from queue 'queue_account_created': {message}");

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

            var (subject, body) = EmailTemplateFactory.BuildWelcomeEmail(user);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                LogHelper.LogInfo(_logger, $"Sending welcome email to {user.Email}");
                await emailService.SendEmailAsync(
                    to: user.Email,
                    subject: subject,
                    body: body
                );
                LogHelper.LogInfo(_logger, $"Welcome email successfully sent to {user.Email}.");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, $"Failed to send welcome email to {user.Email}.");
            }
        });
    }
}
