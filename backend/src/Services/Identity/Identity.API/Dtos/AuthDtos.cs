namespace Identity.API.Dtos;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string ConfirmPassword, string UserName, string Role = "Customer");
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiration);
public record UserResponse(string Id, string Email, string FirstName, string LastName, string UserName, IList<string> Roles);
public record UpdateProfileRequest(string FirstName, string LastName, string Email);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword, string ConfirmPassword);

public record AddressRequest(string Label, string FirstName, string LastName, string AddressLine, string Country, string State, string ZipCode, string EmailAddress, bool IsDefault);
public record AddressResponse(Guid Id, string Label, string FirstName, string LastName, string AddressLine, string Country, string State, string ZipCode, string EmailAddress, bool IsDefault);

public record PaymentMethodRequest(string Label, string CardName, string CardNumber, string Expiration, bool IsDefault);
public record PaymentMethodResponse(Guid Id, string Label, string CardName, string CardNumberLast4, string Expiration, bool IsDefault);
