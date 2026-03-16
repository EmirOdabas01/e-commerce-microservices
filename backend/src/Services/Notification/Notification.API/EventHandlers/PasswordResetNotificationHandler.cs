using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.EventHandlers;

public class PasswordResetNotificationHandler : IConsumer<PasswordResetEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetNotificationHandler> _logger;

    public PasswordResetNotificationHandler(IEmailService emailService, ILogger<PasswordResetNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PasswordResetEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Sending password reset email to {Email}", message.Email);

        await _emailService.SendEmailAsync(
            message.Email,
            "Password Reset Request",
            $"You requested a password reset.\n\nYour reset token: {message.ResetToken}\n\nIf you didn't request this, please ignore this email.");
    }
}
