using MediatR;
using Order.Application.Commands.CancelOrder;
using Order.Application.Commands.CreateOrder;
using Order.Application.Commands.DeleteOrder;
using Order.Application.Commands.RefundOrder;
using Order.Application.Commands.UpdateOrder;
using Order.Application.Queries.GetOrders;
using Order.Application.Queries.GetOrdersByUser;

namespace Order.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapGet("/", async (int? pageNumber, int? pageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersQuery(pageNumber ?? 1, pageSize ?? 10));
            return Results.Ok(result);
        })
        .WithName("GetOrders");

        group.MapGet("/user/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByUserQuery(userName));
            return Results.Ok(result);
        })
        .WithName("GetOrdersByUser");

        group.MapPost("/", async (CreateOrderCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/orders/{result.Id}", result);
        })
        .WithName("CreateOrder");

        group.MapPut("/", async (UpdateOrderCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateOrder");

        group.MapPut("/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("CancelOrder");

        group.MapPut("/{id:guid}/refund", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new RefundOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("RefundOrder");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteOrder");
    }
}
