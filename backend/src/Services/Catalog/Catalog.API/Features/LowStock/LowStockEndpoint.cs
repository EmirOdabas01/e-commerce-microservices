using Carter;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Features.LowStock;

public record LowStockProductResponse(Guid Id, string Name, int Stock);

public class LowStockEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/low-stock", async (int? threshold, IDocumentSession session) =>
        {
            var limit = threshold ?? 5;

            var products = await session.Query<Product>()
                .Where(p => p.Stock <= limit)
                .OrderBy(p => p.Stock)
                .ToListAsync();

            return Results.Ok(products.Select(p =>
                new LowStockProductResponse(p.Id, p.Name, p.Stock)));
        })
        .WithName("GetLowStockProducts")
        .RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
