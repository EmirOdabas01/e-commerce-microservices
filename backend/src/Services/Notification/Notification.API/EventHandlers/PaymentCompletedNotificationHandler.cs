using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.EventHandlers;

public class PaymentCompletedNotificationHandler : IConsumer<PaymentCompletedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentCompletedNotificationHandler> _logger;

    public PaymentCompletedNotificationHandler(IEmailService emailService, ILogger<PaymentCompletedNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Sending payment confirmation for order {OrderId}", message.OrderId);

        await _emailService.SendEmailAsync(
            message.EmailAddress,
            $"Payment Confirmed - Order {message.OrderId}",
            $"Dear {message.CustomerName},\n\nYour payment of ${message.Amount:F2} has been processed successfully.\nTransaction ID: {message.TransactionId}");
    }
}
