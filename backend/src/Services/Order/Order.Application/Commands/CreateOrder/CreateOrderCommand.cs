using MediatR;

namespace Order.Application.Commands.CreateOrder;

public record CreateOrderItem(Guid ProductId, string ProductName, decimal Price, int Quantity);

public record CreateOrderCommand(
    string UserName,
    string FirstName,
    string LastName,
    string EmailAddress,
    string AddressLine,
    string Country,
    string State,
    string ZipCode,
    string CardName,
    string CardNumber,
    string Expiration,
    string Cvv,
    int PaymentMethod,
    List<CreateOrderItem> Items) : IRequest<CreateOrderResult>;

public record CreateOrderResult(Guid Id);
