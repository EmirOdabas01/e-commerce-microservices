using Carter;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Features.Coupons;

public record CouponRequest(string Code, decimal DiscountPercent, decimal? MaxDiscountAmount, decimal MinOrderAmount, int UsageLimit, DateTime ExpiresAt);
public record CouponResponse(Guid Id, string Code, decimal DiscountPercent, decimal? MaxDiscountAmount, decimal MinOrderAmount, int UsageLimit, int UsedCount, DateTime ExpiresAt, bool IsActive);
public record ValidateCouponRequest(string Code, decimal OrderTotal);
public record ValidateCouponResponse(bool IsValid, decimal DiscountAmount, string? Message);

public class CouponEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/coupons", async (IDocumentSession session) =>
        {
            var coupons = await session.Query<Coupon>()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Results.Ok(coupons.Select(c =>
                new CouponResponse(c.Id, c.Code, c.DiscountPercent, c.MaxDiscountAmount, c.MinOrderAmount, c.UsageLimit, c.UsedCount, c.ExpiresAt, c.IsActive)));
        })
        .WithName("GetCoupons")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        app.MapPost("/api/coupons", async (CouponRequest request, IDocumentSession session) =>
        {
            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpperInvariant(),
                DiscountPercent = request.DiscountPercent,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinOrderAmount = request.MinOrderAmount,
                UsageLimit = request.UsageLimit,
                ExpiresAt = request.ExpiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(coupon);
            await session.SaveChangesAsync();

            return Results.Created($"/api/coupons/{coupon.Id}",
                new CouponResponse(coupon.Id, coupon.Code, coupon.DiscountPercent, coupon.MaxDiscountAmount, coupon.MinOrderAmount, coupon.UsageLimit, coupon.UsedCount, coupon.ExpiresAt, coupon.IsActive));
        })
        .WithName("CreateCoupon")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        app.MapDelete("/api/coupons/{id:guid}", async (Guid id, IDocumentSession session) =>
        {
            var coupon = await session.LoadAsync<Coupon>(id);
            if (coupon is null) return Results.NotFound();

            session.Delete(coupon);
            await session.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteCoupon")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        app.MapPost("/api/coupons/validate", async (ValidateCouponRequest request, IDocumentSession session) =>
        {
            var coupon = await session.Query<Coupon>()
                .FirstOrDefaultAsync(c => c.Code == request.Code.ToUpperInvariant());

            if (coupon is null)
                return Results.Ok(new ValidateCouponResponse(false, 0, "Coupon not found."));

            if (!coupon.IsActive)
                return Results.Ok(new ValidateCouponResponse(false, 0, "Coupon is no longer active."));

            if (coupon.ExpiresAt < DateTime.UtcNow)
                return Results.Ok(new ValidateCouponResponse(false, 0, "Coupon has expired."));

            if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
                return Results.Ok(new ValidateCouponResponse(false, 0, "Coupon usage limit reached."));

            if (request.OrderTotal < coupon.MinOrderAmount)
                return Results.Ok(new ValidateCouponResponse(false, 0, $"Minimum order amount is {coupon.MinOrderAmount:C}."));

            var discount = request.OrderTotal * coupon.DiscountPercent / 100;
            if (coupon.MaxDiscountAmount.HasValue && discount > coupon.MaxDiscountAmount.Value)
                discount = coupon.MaxDiscountAmount.Value;

            coupon.UsedCount++;
            session.Update(coupon);
            await session.SaveChangesAsync();

            return Results.Ok(new ValidateCouponResponse(true, discount, null));
        })
        .WithName("ValidateCoupon");
    }
}
