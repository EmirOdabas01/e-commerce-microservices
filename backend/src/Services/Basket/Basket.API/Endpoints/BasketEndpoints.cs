using Basket.API.Data;
using Basket.API.Dtos;
using Basket.API.Models;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/basket");

        group.MapGet("/{userName}", async (string userName, IBasketRepository repository) =>
        {
            var basket = await repository.GetBasket(userName);
            return basket is null
                ? Results.Ok(new ShoppingCart { UserName = userName })
                : Results.Ok(basket);
        })
        .WithName("GetBasket")
        .Produces<ShoppingCart>();

        group.MapPost("/", async (ShoppingCart basket, IBasketRepository repository) =>
        {
            var result = await repository.StoreBasket(basket);
            return Results.Ok(result);
        })
        .WithName("StoreBasket")
        .Produces<ShoppingCart>();

        group.MapDelete("/{userName}", async (string userName, IBasketRepository repository) =>
        {
            var result = await repository.DeleteBasket(userName);
            return Results.Ok(result);
        })
        .WithName("DeleteBasket")
        .Produces<bool>();

        group.MapPost("/checkout", async (BasketCheckoutDto dto, IBasketRepository repository, IPublishEndpoint publishEndpoint) =>
        {
            var basket = await repository.GetBasket(dto.UserName);
            if (basket is null)
            {
                throw new NotFoundException("Basket", dto.UserName);
            }

            var eventMessage = new BasketCheckoutEvent
            {
                UserName = dto.UserName,
                CustomerId = dto.CustomerId,
                TotalPrice = basket.TotalPrice,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailAddress = dto.EmailAddress,
                AddressLine = dto.AddressLine,
                Country = dto.Country,
                State = dto.State,
                ZipCode = dto.ZipCode,
                CardName = dto.CardName,
                CardNumber = dto.CardNumber,
                Expiration = dto.Expiration,
                Cvv = dto.Cvv,
                PaymentMethod = dto.PaymentMethod
            };

            await publishEndpoint.Publish(eventMessage);
            await repository.DeleteBasket(dto.UserName);

            return Results.Accepted();
        })
        .WithName("CheckoutBasket")
        .Produces(StatusCodes.Status202Accepted);
    }
}
