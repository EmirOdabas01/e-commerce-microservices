using BuildingBlocks.Exceptions;
using MediatR;
using Order.Application.Data;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.UpdateOrder;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, UpdateOrderResult>
{
    private readonly IOrderDbContext _dbContext;

    public UpdateOrderHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync([command.Id], cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), command.Id);
        }

        order.UserName = command.UserName;
        order.ShippingAddress = new Address(
            command.FirstName,
            command.LastName,
            command.EmailAddress,
            command.AddressLine,
            command.Country,
            command.State,
            command.ZipCode);
        order.Status = command.Status;
        order.LastModifiedAt = DateTime.UtcNow;

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }
}
