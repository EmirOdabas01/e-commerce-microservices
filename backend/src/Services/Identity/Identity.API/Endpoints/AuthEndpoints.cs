using Identity.API.Dtos;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity");

        group.MapPost("/register", async (RegisterRequest request, UserManager<AppUser> userManager, ITokenService tokenService) =>
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Results.BadRequest(new[] { new { Code = "PasswordMismatch", Description = "Passwords do not match." } });
            }

            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            var role = request.Role is "Customer" or "Seller" ? request.Role : "Customer";
            await userManager.AddToRoleAsync(user, role);

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateAccessToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken();

            return Results.Ok(new AuthResponse(token, refreshToken, DateTime.UtcNow.AddMinutes(60)));
        })
        .WithName("Register")
        .AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, UserManager<AppUser> userManager, ITokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateAccessToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken();

            return Results.Ok(new AuthResponse(token, refreshToken, DateTime.UtcNow.AddMinutes(60)));
        })
        .WithName("Login")
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshRequest request, ITokenService tokenService) =>
        {
            var newRefreshToken = tokenService.GenerateRefreshToken();
            return Results.Ok(new { RefreshToken = newRefreshToken });
        })
        .WithName("Refresh")
        .AllowAnonymous();

        group.MapGet("/me", async (HttpContext context, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);

            return Results.Ok(new UserResponse(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                user.UserName!,
                roles));
        })
        .WithName("GetCurrentUser")
        .RequireAuthorization();
    }
}
