using System.Security.Claims;
using Carter;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Features.Wishlist;

public record WishlistItemResponse(Guid Id, Guid ProductId, string ProductName, string ImageFile, decimal Price, DateTime CreatedAt);

public class WishlistEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/wishlist", async (HttpContext context, IDocumentSession session) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var items = await session.Query<WishlistItem>()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            var productIds = items.Select(i => i.ProductId).ToList();
            var products = await session.LoadManyAsync<Product>(productIds);
            var productMap = products.Where(p => p != null).ToDictionary(p => p!.Id);

            var result = items.Select(w =>
            {
                productMap.TryGetValue(w.ProductId, out var product);
                return new WishlistItemResponse(
                    w.Id, w.ProductId,
                    product?.Name ?? "Unknown",
                    product?.ImageFile ?? "",
                    product?.Price ?? 0,
                    w.CreatedAt);
            });

            return Results.Ok(result);
        })
        .WithName("GetWishlist")
        .RequireAuthorization();

        app.MapPost("/api/wishlist/{productId:guid}", async (Guid productId, HttpContext context, IDocumentSession session) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var existing = await session.Query<WishlistItem>()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing is not null)
                return Results.Ok(new { Message = "Already in wishlist." });

            var item = new WishlistItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(item);
            await session.SaveChangesAsync();

            return Results.Created($"/api/wishlist", new { item.Id });
        })
        .WithName("AddToWishlist")
        .RequireAuthorization();

        app.MapDelete("/api/wishlist/{productId:guid}", async (Guid productId, HttpContext context, IDocumentSession session) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var item = await session.Query<WishlistItem>()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (item is null) return Results.NotFound();

            session.Delete(item);
            await session.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("RemoveFromWishlist")
        .RequireAuthorization();
    }
}
