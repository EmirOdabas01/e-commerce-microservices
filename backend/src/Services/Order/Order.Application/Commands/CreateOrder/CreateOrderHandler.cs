using MediatR;
using Order.Application.Data;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderDbContext _dbContext;

    public CreateOrderHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
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
        order.TotalPrice = command.TotalPrice;

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id);
    }
}
