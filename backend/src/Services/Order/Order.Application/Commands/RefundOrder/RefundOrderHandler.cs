using BuildingBlocks.Exceptions;
using MediatR;
using Order.Application.Data;
using Order.Domain.Enums;

namespace Order.Application.Commands.RefundOrder;

public record RefundOrderCommand(Guid Id) : IRequest<RefundOrderResult>;
public record RefundOrderResult(bool IsSuccess);

public class RefundOrderHandler : IRequestHandler<RefundOrderCommand, RefundOrderResult>
{
    private readonly IOrderDbContext _dbContext;

    public RefundOrderHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefundOrderResult> Handle(RefundOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync([command.Id], cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), command.Id);
        }

        if (order.Status != OrderStatus.Completed)
        {
            throw new BadRequestException($"Can only refund completed orders. Current status: {order.Status}.");
        }

        order.Status = OrderStatus.Refunded;
        order.LastModifiedAt = DateTime.UtcNow;

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RefundOrderResult(true);
    }
}
