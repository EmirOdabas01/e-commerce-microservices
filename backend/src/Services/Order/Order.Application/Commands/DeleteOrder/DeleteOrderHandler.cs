using BuildingBlocks.Exceptions;
using MediatR;
using Order.Application.Data;

namespace Order.Application.Commands.DeleteOrder;

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, DeleteOrderResult>
{
    private readonly IOrderDbContext _dbContext;

    public DeleteOrderHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync([command.Id], cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), command.Id);
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteOrderResult(true);
    }
}
