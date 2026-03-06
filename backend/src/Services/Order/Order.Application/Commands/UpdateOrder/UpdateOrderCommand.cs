using MediatR;
using Order.Domain.Enums;

namespace Order.Application.Commands.UpdateOrder;

public record UpdateOrderCommand(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string EmailAddress,
    string AddressLine,
    string Country,
    string State,
    string ZipCode,
    OrderStatus Status) : IRequest<UpdateOrderResult>;

public record UpdateOrderResult(bool IsSuccess);
