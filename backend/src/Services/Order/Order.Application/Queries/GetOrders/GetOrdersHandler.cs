using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly IOrderDbContext _dbContext;

    public GetOrdersHandler(IOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await _dbContext.Orders.LongCountAsync(cancellationToken);

        var orders = await _dbContext.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var orderDtos = orders.Adapt<List<OrderDto>>();
        return new GetOrdersResult(orderDtos, totalCount, query.PageNumber - 1, query.PageSize);
    }
}
