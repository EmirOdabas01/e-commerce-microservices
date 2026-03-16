using Identity.API.Data;
using Identity.API.Dtos;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity/addresses").RequireAuthorization();

        group.MapGet("/", async (HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            var addresses = await db.Addresses
                .Where(a => a.UserId == user.Id)
                .Select(a => new AddressResponse(a.Id, a.Label, a.FirstName, a.LastName, a.AddressLine, a.Country, a.State, a.ZipCode, a.EmailAddress, a.IsDefault))
                .ToListAsync();

            return Results.Ok(addresses);
        })
        .WithName("GetAddresses");

        group.MapPost("/", async (AddressRequest request, HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            if (request.IsDefault)
            {
                await db.Addresses.Where(a => a.UserId == user.Id && a.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));
            }

            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Label = request.Label,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AddressLine = request.AddressLine,
                Country = request.Country,
                State = request.State,
                ZipCode = request.ZipCode,
                EmailAddress = request.EmailAddress,
                IsDefault = request.IsDefault
            };

            db.Addresses.Add(address);
            await db.SaveChangesAsync();

            return Results.Created($"/api/identity/addresses/{address.Id}",
                new AddressResponse(address.Id, address.Label, address.FirstName, address.LastName, address.AddressLine, address.Country, address.State, address.ZipCode, address.EmailAddress, address.IsDefault));
        })
        .WithName("CreateAddress");

        group.MapPut("/{id:guid}", async (Guid id, AddressRequest request, HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (address is null) return Results.NotFound();

            if (request.IsDefault)
            {
                await db.Addresses.Where(a => a.UserId == user.Id && a.IsDefault && a.Id != id)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));
            }

            address.Label = request.Label;
            address.FirstName = request.FirstName;
            address.LastName = request.LastName;
            address.AddressLine = request.AddressLine;
            address.Country = request.Country;
            address.State = request.State;
            address.ZipCode = request.ZipCode;
            address.EmailAddress = request.EmailAddress;
            address.IsDefault = request.IsDefault;

            await db.SaveChangesAsync();

            return Results.Ok(new AddressResponse(address.Id, address.Label, address.FirstName, address.LastName, address.AddressLine, address.Country, address.State, address.ZipCode, address.EmailAddress, address.IsDefault));
        })
        .WithName("UpdateAddress");

        group.MapDelete("/{id:guid}", async (Guid id, HttpContext context, UserManager<AppUser> userManager, IdentityDbContext db) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null) return Results.Unauthorized();

            var deleted = await db.Addresses.Where(a => a.Id == id && a.UserId == user.Id).ExecuteDeleteAsync();

            return deleted > 0 ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteAddress");
    }
}
