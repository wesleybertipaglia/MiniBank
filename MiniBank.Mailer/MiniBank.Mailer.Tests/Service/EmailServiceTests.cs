using MiniBank.Mailer.Application.Service;
using MiniBank.Mailer.Core.Interface;

namespace MiniBank.Mailer.Tests.Service;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_WritesToConsoleAndCompletes()
    {
        // Arrange
        IEmailService emailService = new EmailService();
        string to = "test@example.com";
        string subject = "Test Subject";
        string body = "This is a test body";

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => emailService.SendEmailAsync(to, subject, body));
        
        Assert.Null(exception);
    }
}