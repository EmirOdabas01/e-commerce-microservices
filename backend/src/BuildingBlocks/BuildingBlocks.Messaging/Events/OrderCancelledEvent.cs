namespace BuildingBlocks.Messaging.Events;

public record OrderCancelledItem(Guid ProductId, int Quantity);

public record OrderCancelledEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public List<OrderCancelledItem> Items { get; init; } = [];
}
