using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Domain.Events;

namespace Order.API.EventHandlers;

public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IOrderDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCreatedDomainEventHandler> _logger;

    public OrderCreatedDomainEventHandler(
        IOrderDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCreatedDomainEventHandler> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == notification.OrderId, cancellationToken);

        if (order is null) return;

        var integrationEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserName = order.UserName,
            TotalPrice = order.TotalPrice,
            FirstName = order.ShippingAddress.FirstName,
            LastName = order.ShippingAddress.LastName,
            EmailAddress = order.ShippingAddress.EmailAddress,
            AddressLine = order.ShippingAddress.AddressLine,
            Country = order.ShippingAddress.Country,
            State = order.ShippingAddress.State,
            ZipCode = order.ShippingAddress.ZipCode,
            CardName = order.PaymentInfo.CardName,
            CardNumber = order.PaymentInfo.CardNumber,
            Expiration = order.PaymentInfo.Expiration,
            Cvv = order.PaymentInfo.Cvv,
            PaymentMethod = order.PaymentInfo.PaymentMethod
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        _logger.LogInformation("Published OrderCreatedEvent for order {OrderId}", order.Id);
    }
}
