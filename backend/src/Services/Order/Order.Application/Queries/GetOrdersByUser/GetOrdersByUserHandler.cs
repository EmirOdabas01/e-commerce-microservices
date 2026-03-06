using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrdersByUser;

public class GetOrdersByUserHandler : IRequestHandler<GetOrdersByUserQuery, GetOrdersByUserResult>
{
    private readonly IOrderDbContext _dbContext;

    public GetOrdersByUserHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrdersByUserResult> Handle(GetOrdersByUserQuery query, CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.UserName == query.UserName)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var orderDtos = orders.Adapt<List<OrderDto>>();
        return new GetOrdersByUserResult(orderDtos);
    }
}
