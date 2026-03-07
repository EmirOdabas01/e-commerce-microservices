using MediatR;
using Order.Application.Data;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderDbContext _dbContext;
    private readonly IPublisher _publisher;

    public CreateOrderHandler(IOrderDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var address = new Address(
            command.FirstName,
            command.LastName,
            command.EmailAddress,
            command.AddressLine,
            command.Country,
            command.State,
            command.ZipCode);

        var payment = new Payment(
            command.CardName,
            command.CardNumber,
            command.Expiration,
            command.Cvv,
            command.PaymentMethod);

        var order = Domain.Models.Order.Create(command.UserName, address, payment);

        foreach (var item in command.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        order.ClearDomainEvents();

        return new CreateOrderResult(order.Id);
    }
}
