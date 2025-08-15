using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace new_app.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        _logger.LogInformation($"Email would be sent to {email} with subject: {subject}");
        // In a real implementation, you would connect to an email service here
        return Task.CompletedTask;
    }
}