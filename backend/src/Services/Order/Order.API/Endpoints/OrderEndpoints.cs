using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
        .WithName("GetOrders")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapGet("/user/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByUserQuery(userName));
            return Results.Ok(result);
        })
        .WithName("GetOrdersByUser")
        .RequireAuthorization();

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
        .WithName("CancelOrder")
        .RequireAuthorization();

        group.MapPut("/{id:guid}/refund", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new RefundOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("RefundOrder")
        .RequireAuthorization();

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteOrderCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteOrder")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapGet("/{id:guid}/invoice", async (Guid id, IOrderDbContext dbContext) =>
        {
            var order = await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order is null) return Results.NotFound();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Header().Text("INVOICE").FontSize(24).Bold().AlignCenter();

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().Text($"Order ID: {order.Id}").FontSize(10);
                        col.Item().Text($"Date: {order.CreatedAt:yyyy-MM-dd}").FontSize(10);
                        col.Item().Text($"Customer: {order.ShippingAddress.FirstName} {order.ShippingAddress.LastName}").FontSize(10);
                        col.Item().Text($"Email: {order.ShippingAddress.EmailAddress}").FontSize(10);
                        col.Item().Text($"Address: {order.ShippingAddress.AddressLine}, {order.ShippingAddress.State}, {order.ShippingAddress.Country} {order.ShippingAddress.ZipCode}").FontSize(10);

                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Product").Bold();
                                header.Cell().Text("Qty").Bold();
                                header.Cell().Text("Price").Bold();
                                header.Cell().Text("Total").Bold();
                            });

                            foreach (var item in order.Items)
                            {
                                table.Cell().Text(item.ProductName);
                                table.Cell().Text(item.Quantity.ToString());
                                table.Cell().Text($"${item.Price:F2}");
                                table.Cell().Text($"${item.Price * item.Quantity:F2}");
                            }
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().AlignRight().Text($"Total: ${order.TotalPrice:F2}").FontSize(14).Bold();
                        col.Item().AlignRight().Text($"Status: {order.Status}").FontSize(10);
                    });

                    page.Footer().AlignCenter().Text("Thank you for your purchase!").FontSize(9);
                });
            });

            var bytes = pdf.GeneratePdf();

            return Results.File(bytes, "application/pdf", $"invoice-{order.Id}.pdf");
        })
        .WithName("GetOrderInvoice")
        .RequireAuthorization();

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
