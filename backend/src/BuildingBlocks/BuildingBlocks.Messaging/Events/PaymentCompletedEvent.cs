namespace BuildingBlocks.Messaging.Events;

public record PaymentCompletedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public string TransactionId { get; init; } = default!;
}
