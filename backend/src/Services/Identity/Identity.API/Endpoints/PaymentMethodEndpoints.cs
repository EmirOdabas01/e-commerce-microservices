using Identity.API.Data;
using Identity.API.Dtos;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Endpoints;

public static class PaymentMethodEndpoints
{
    public static void MapPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity/payment-methods").RequireAuthorization();

        group.MapGet("/", async (HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            var methods = await db.PaymentMethods
                .Where(p => p.UserId == user.Id)
                .Select(p => new PaymentMethodResponse(p.Id, p.Label, p.CardName, p.CardNumberLast4, p.Expiration, p.IsDefault))
                .ToListAsync();

            return Results.Ok(methods);
        })
        .WithName("GetPaymentMethods");

        group.MapPost("/", async (PaymentMethodRequest request, HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            if (request.IsDefault)
            {
                await db.PaymentMethods.Where(p => p.UserId == user.Id && p.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));
            }

            var last4 = request.CardNumber.Length >= 4
                ? request.CardNumber[^4..]
                : request.CardNumber;

            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Label = request.Label,
                CardName = request.CardName,
                CardNumberLast4 = last4,
                Expiration = request.Expiration,
                IsDefault = request.IsDefault
            };

            db.PaymentMethods.Add(method);
            await db.SaveChangesAsync();

            return Results.Created($"/api/identity/payment-methods/{method.Id}",
                new PaymentMethodResponse(method.Id, method.Label, method.CardName, method.CardNumberLast4, method.Expiration, method.IsDefault));
        })
        .WithName("CreatePaymentMethod");

        group.MapDelete("/{id:guid}", async (Guid id, HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            var deleted = await db.PaymentMethods.Where(p => p.Id == id && p.UserId == user.Id).ExecuteDeleteAsync();

            return deleted > 0 ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeletePaymentMethod");
    }
}
