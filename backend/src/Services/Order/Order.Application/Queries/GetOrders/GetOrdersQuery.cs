using MediatR;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrders;

public record GetOrdersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<GetOrdersResult>;
public record GetOrdersResult(IEnumerable<OrderDto> Orders);
