using MiniBank.Mailer.Core.Interface;

namespace MiniBank.Mailer.Application.Service;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"Sending e-mail to: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");
        return Task.CompletedTask;
    }
}