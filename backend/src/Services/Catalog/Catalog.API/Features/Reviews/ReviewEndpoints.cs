using System.Security.Claims;
using Carter;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Features.Reviews;

public record CreateReviewRequest(Guid ProductId, int Rating, string Text);
public record ReviewResponse(Guid Id, Guid ProductId, string UserId, string UserName, int Rating, string Text, DateTime CreatedAt);
public record ProductRatingSummary(double AverageRating, int ReviewCount);

public class ReviewEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{productId:guid}/reviews", async (Guid productId, IDocumentSession session) =>
        {
            var reviews = await session.Query<Review>()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Results.Ok(reviews.Select(r =>
                new ReviewResponse(r.Id, r.ProductId, r.UserId, r.UserName, r.Rating, r.Text, r.CreatedAt)));
        })
        .WithName("GetProductReviews");

        app.MapGet("/api/products/{productId:guid}/rating", async (Guid productId, IDocumentSession session) =>
        {
            var reviews = await session.Query<Review>()
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviews.Count == 0)
                return Results.Ok(new ProductRatingSummary(0, 0));

            return Results.Ok(new ProductRatingSummary(reviews.Average(r => r.Rating), reviews.Count));
        })
        .WithName("GetProductRating");

        app.MapPost("/api/products/reviews", async (CreateReviewRequest request, HttpContext context, IDocumentSession session) =>
        {
            if (request.Rating < 1 || request.Rating > 5)
                return Results.BadRequest(new { Message = "Rating must be between 1 and 5." });

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userName = context.User.FindFirstValue(ClaimTypes.Name)!;

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                UserId = userId,
                UserName = userName,
                Rating = request.Rating,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(review);
            await session.SaveChangesAsync();

            return Results.Created($"/api/products/{review.ProductId}/reviews",
                new ReviewResponse(review.Id, review.ProductId, review.UserId, review.UserName, review.Rating, review.Text, review.CreatedAt));
        })
        .WithName("CreateReview")
        .RequireAuthorization();

        app.MapPost("/api/products/reviews/{id:guid}/report", async (Guid id, HttpContext context, IDocumentSession session) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var review = await session.LoadAsync<Review>(id);

            if (review is null) return Results.NotFound();

            var report = new ReviewReport
            {
                Id = Guid.NewGuid(),
                ReviewId = id,
                ReportedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(report);
            await session.SaveChangesAsync();

            return Results.Ok(new { Message = "Review reported." });
        })
        .WithName("ReportReview")
        .RequireAuthorization();

        app.MapDelete("/api/products/reviews/{id:guid}", async (Guid id, HttpContext context, IDocumentSession session) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = context.User.IsInRole("Admin");

            var review = await session.LoadAsync<Review>(id);
            if (review is null) return Results.NotFound();

            if (!isAdmin && review.UserId != userId)
                return Results.Forbid();

            session.Delete(review);
            await session.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteReview")
        .RequireAuthorization();
    }
}
