namespace BuildingBlocks.Messaging.Events;

public record PaymentFailedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = default!;
}
