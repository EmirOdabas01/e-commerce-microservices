using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Payment.API.EventHandlers;

public class OrderCreatedEventHandler : IConsumer<OrderCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Processing payment for order {OrderId}", context.Message.OrderId);

        var isPaymentSuccessful = SimulatePayment(context.Message);

        if (isPaymentSuccessful)
        {
            _logger.LogInformation("Payment succeeded for order {OrderId}", context.Message.OrderId);

            await _publishEndpoint.Publish(new PaymentCompletedEvent
            {
                OrderId = context.Message.OrderId,
                Amount = context.Message.TotalPrice,
                TransactionId = Guid.NewGuid().ToString()
            });
        }
        else
        {
            _logger.LogWarning("Payment failed for order {OrderId}", context.Message.OrderId);

            await _publishEndpoint.Publish(new PaymentFailedEvent
            {
                OrderId = context.Message.OrderId,
                Reason = "Insufficient funds"
            });
        }
    }

    private static bool SimulatePayment(OrderCreatedEvent order)
    {
        return order.TotalPrice < 10000;
    }
}
