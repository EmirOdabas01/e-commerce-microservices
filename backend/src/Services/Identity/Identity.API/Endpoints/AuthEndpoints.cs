using BuildingBlocks.Messaging.Events;
using Identity.API.Dtos;
using Identity.API.Models;
using Identity.API.Services;
using MassTransit;
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

        group.MapPut("/profile", async (UpdateProfileRequest request, HttpContext context, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
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
        .WithName("UpdateProfile")
        .RequireAuthorization();

        group.MapPost("/logout", async (HttpContext context, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            await userManager.UpdateSecurityStampAsync(user);

            return Results.Ok(new { Message = "Logged out successfully." });
        })
        .WithName("Logout")
        .RequireAuthorization();

        group.MapPost("/forgot-password", async (ForgotPasswordRequest request, UserManager<AppUser> userManager, IPublishEndpoint publishEndpoint) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Results.Ok(new { Message = "If the email exists, a reset token has been generated." });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            await publishEndpoint.Publish(new PasswordResetEvent
            {
                Email = request.Email,
                ResetToken = token
            });

            return Results.Ok(new { Token = token, Message = "Use this token to reset your password." });
        })
        .WithName("ForgotPassword")
        .AllowAnonymous();

        group.MapPost("/reset-password", async (ResetPasswordRequest request, UserManager<AppUser> userManager) =>
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Results.BadRequest(new { Message = "Passwords do not match." });
            }

            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Results.BadRequest(new { Message = "Invalid request." });
            }

            var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(new { Message = "Password has been reset successfully." });
        })
        .WithName("ResetPassword")
        .AllowAnonymous();
    }
}
