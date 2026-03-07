namespace Identity.API.Dtos;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string ConfirmPassword, string UserName, string Role = "Customer");
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiration);
public record UserResponse(string Id, string Email, string FirstName, string LastName, string UserName, IList<string> Roles);
