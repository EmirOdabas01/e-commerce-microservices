using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Domain.Enums;

namespace Order.API.EventHandlers;

public class PaymentCompletedEventHandler : IConsumer<PaymentCompletedEvent>
{
    private readonly IOrderDbContext _dbContext;
    private readonly ILogger<PaymentCompletedEventHandler> _logger;

    public PaymentCompletedEventHandler(IOrderDbContext dbContext, ILogger<PaymentCompletedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId, context.CancellationToken);

        if (order is null) return;

        order.Status = OrderStatus.Completed;
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Order {OrderId} marked as Completed", order.Id);
    }
}
