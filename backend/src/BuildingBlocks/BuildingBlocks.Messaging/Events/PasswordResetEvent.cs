namespace BuildingBlocks.Messaging.Events;

public record PasswordResetEvent : IntegrationEvent
{
    public string Email { get; init; } = default!;
    public string ResetToken { get; init; } = default!;
}
