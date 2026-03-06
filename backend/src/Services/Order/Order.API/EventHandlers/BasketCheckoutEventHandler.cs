using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Order.Application.Commands.CreateOrder;

namespace Order.API.EventHandlers;

public class BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<BasketCheckoutEventHandler> _logger;

    public BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        _logger.LogInformation("Processing basket checkout for user {UserName}", context.Message.UserName);

        var command = new CreateOrderCommand(
            context.Message.UserName,
            context.Message.TotalPrice,
            context.Message.FirstName,
            context.Message.LastName,
            context.Message.EmailAddress,
            context.Message.AddressLine,
            context.Message.Country,
            context.Message.State,
            context.Message.ZipCode,
            context.Message.CardName,
            context.Message.CardNumber,
            context.Message.Expiration,
            context.Message.Cvv,
            context.Message.PaymentMethod);

        var result = await _sender.Send(command);

        _logger.LogInformation("Order {OrderId} created from basket checkout", result.Id);
    }
}
