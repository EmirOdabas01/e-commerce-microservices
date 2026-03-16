using BuildingBlocks.Exceptions;
using MediatR;
using Order.Application.Data;
using Order.Domain.Enums;

namespace Order.Application.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest<CancelOrderResult>;
public record CancelOrderResult(bool IsSuccess);

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, CancelOrderResult>
{
    private readonly IOrderDbContext _dbContext;

    public CancelOrderHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CancelOrderResult> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync([command.Id], cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), command.Id);
        }

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Processing))
        {
            throw new BadRequestException($"Cannot cancel order with status {order.Status}.");
        }

        order.Status = OrderStatus.Cancelled;
        order.LastModifiedAt = DateTime.UtcNow;

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CancelOrderResult(true);
    }
}
