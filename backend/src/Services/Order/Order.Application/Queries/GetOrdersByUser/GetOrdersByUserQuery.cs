using MediatR;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrdersByUser;

public record GetOrdersByUserQuery(string UserName) : IRequest<GetOrdersByUserResult>;
public record GetOrdersByUserResult(IEnumerable<OrderDto> Orders);
