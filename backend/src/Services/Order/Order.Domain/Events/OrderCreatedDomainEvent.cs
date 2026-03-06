using MediatR;

namespace Order.Domain.Events;

public record OrderCreatedDomainEvent(Guid OrderId) : INotification;
