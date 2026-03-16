using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.EventHandlers;

public class OrderCreatedNotificationHandler : IConsumer<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCreatedNotificationHandler> _logger;

    public OrderCreatedNotificationHandler(IEmailService emailService, ILogger<OrderCreatedNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Sending order confirmation email for order {OrderId}", message.OrderId);

        await _emailService.SendEmailAsync(
            message.EmailAddress,
            $"Order Confirmation - {message.OrderId}",
            $"Dear {message.FirstName},\n\nYour order #{message.OrderId} has been placed successfully.\nTotal: ${message.TotalPrice:F2}\n\nThank you for your purchase!");
    }
}
