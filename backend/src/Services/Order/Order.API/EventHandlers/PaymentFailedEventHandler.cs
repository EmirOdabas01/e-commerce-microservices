using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Domain.Enums;

namespace Order.API.EventHandlers;

public class PaymentFailedEventHandler : IConsumer<PaymentFailedEvent>
{
    private readonly IOrderDbContext _dbContext;
    private readonly ILogger<PaymentFailedEventHandler> _logger;

    public PaymentFailedEventHandler(IOrderDbContext dbContext, ILogger<PaymentFailedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId, context.CancellationToken);

        if (order is null) return;

        order.Status = OrderStatus.Failed;
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Order {OrderId} marked as Failed. Reason: {Reason}", order.Id, context.Message.Reason);
    }
}
