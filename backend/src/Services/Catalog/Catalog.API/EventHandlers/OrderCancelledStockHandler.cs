using BuildingBlocks.Messaging.Events;
using Catalog.API.Models;
using Marten;
using MassTransit;

namespace Catalog.API.EventHandlers;

public class OrderCancelledStockHandler : IConsumer<OrderCancelledEvent>
{
    private readonly IDocumentSession _session;
    private readonly ILogger<OrderCancelledStockHandler> _logger;

    public OrderCancelledStockHandler(IDocumentSession session, ILogger<OrderCancelledStockHandler> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;

        foreach (var item in message.Items)
        {
            var product = await _session.LoadAsync<Product>(item.ProductId);
            if (product is null) continue;

            product.Stock += item.Quantity;
            _session.Update(product);

            _logger.LogInformation("Restored stock for {ProductId} by {Quantity}. New stock: {Stock}",
                item.ProductId, item.Quantity, product.Stock);
        }

        await _session.SaveChangesAsync();
    }
}
