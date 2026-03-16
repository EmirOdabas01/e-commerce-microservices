using Identity.API.Dtos;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity/admin")
            .RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapGet("/users", async (UserManager<AppUser> userManager) =>
        {
            var users = await userManager.Users.ToListAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.LockoutEnd,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                    Roles = roles
                });
            }

            return Results.Ok(result);
        })
        .WithName("GetAllUsers");

        group.MapPut("/users/{id}/suspend", async (string id, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return Results.NotFound();

            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(30));

            return Results.Ok(new { Message = "User suspended for 30 days." });
        })
        .WithName("SuspendUser");

        group.MapPut("/users/{id}/unsuspend", async (string id, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return Results.NotFound();

            await userManager.SetLockoutEndDateAsync(user, null);

            return Results.Ok(new { Message = "User unsuspended." });
        })
        .WithName("UnsuspendUser");
    }
}
