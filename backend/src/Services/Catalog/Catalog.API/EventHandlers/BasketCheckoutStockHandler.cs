using BuildingBlocks.Messaging.Events;
using Catalog.API.Models;
using Marten;
using MassTransit;

namespace Catalog.API.EventHandlers;

public class BasketCheckoutStockHandler : IConsumer<BasketCheckoutEvent>
{
    private readonly IDocumentSession _session;
    private readonly ILogger<BasketCheckoutStockHandler> _logger;

    public BasketCheckoutStockHandler(IDocumentSession session, ILogger<BasketCheckoutStockHandler> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var message = context.Message;

        foreach (var item in message.Items)
        {
            var product = await _session.LoadAsync<Product>(item.ProductId);
            if (product is null) continue;

            product.Stock = Math.Max(0, product.Stock - item.Quantity);
            _session.Update(product);

            _logger.LogInformation("Decreased stock for {ProductId} by {Quantity}. New stock: {Stock}",
                item.ProductId, item.Quantity, product.Stock);

            if (product.Stock <= 5)
            {
                _logger.LogWarning("LOW STOCK ALERT: {ProductName} (ID: {ProductId}) has only {Stock} units left",
                    product.Name, product.Id, product.Stock);
            }
        }

        await _session.SaveChangesAsync();
    }
}
