using MediatR;

namespace Order.Application.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid Id) : IRequest<DeleteOrderResult>;
public record DeleteOrderResult(bool IsSuccess);
