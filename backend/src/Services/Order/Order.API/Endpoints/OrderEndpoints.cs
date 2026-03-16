using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Commands.CancelOrder;
using Order.Application.Commands.CreateOrder;
using Order.Application.Commands.DeleteOrder;
using Order.Application.Commands.RefundOrder;
using Order.Application.Commands.UpdateOrder;
using Order.Application.Data;
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

        group.MapPut("/{id:guid}/cancel", async (Guid id, ISender sender, IOrderDbContext dbContext, IPublishEndpoint publishEndpoint) =>
        {
            var order = await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            var result = await sender.Send(new CancelOrderCommand(id));

            if (result.IsSuccess && order is not null)
            {
                await publishEndpoint.Publish(new OrderCancelledEvent
                {
                    OrderId = id,
                    Items = order.Items.Select(i => new OrderCancelledItem(i.ProductId, i.Quantity)).ToList()
                });
            }

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

        group.MapGet("/analytics", async (IOrderDbContext dbContext) =>
        {
            var orders = await dbContext.Orders.Include(o => o.Items).ToListAsync();
            var completedOrders = orders.Where(o => o.Status == Order.Domain.Enums.OrderStatus.Completed).ToList();

            var totalOrders = orders.Count;
            var totalRevenue = completedOrders.Sum(o => o.TotalPrice);
            var averageOrderValue = completedOrders.Count > 0 ? completedOrders.Average(o => o.TotalPrice) : 0;
            var pendingOrders = orders.Count(o => o.Status == Order.Domain.Enums.OrderStatus.Pending);
            var cancelledOrders = orders.Count(o => o.Status == Order.Domain.Enums.OrderStatus.Cancelled);

            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new { g.Key.ProductId, g.Key.ProductName, TotalQuantity = g.Sum(i => i.Quantity), TotalRevenue = g.Sum(i => i.Price * i.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            var statusBreakdown = orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToList();

            return Results.Ok(new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                PendingOrders = pendingOrders,
                CancelledOrders = cancelledOrders,
                TopProducts = topProducts,
                StatusBreakdown = statusBreakdown
            });
        })
        .WithName("GetOrderAnalytics")
        .RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
