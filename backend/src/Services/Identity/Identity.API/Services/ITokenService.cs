using Identity.API.Models;

namespace Identity.API.Services;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
}
