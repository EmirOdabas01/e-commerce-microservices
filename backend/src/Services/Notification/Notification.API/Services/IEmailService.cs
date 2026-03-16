namespace Notification.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Smtp:Host"];

        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogInformation("EMAIL (simulated) To: {To} Subject: {Subject} Body: {Body}", to, subject, body);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Sending email to {To} via {SmtpHost}", to, smtpHost);
        return Task.CompletedTask;
    }
}
