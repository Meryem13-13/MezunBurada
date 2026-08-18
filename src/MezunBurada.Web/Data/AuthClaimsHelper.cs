using System.Security.Claims;
using MezunBurada.Web.Models;

namespace MezunBurada.Web.Data;

// Builds the claims identity used at sign-in (Giriş and Kayıt) so both places stay in sync.
public static class AuthClaimsHelper
{
    public static ClaimsIdentity BuildIdentity(User user, string authenticationScheme)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
        };

        if (user.IsAdmin)
        {
            claims.Add(new Claim("IsAdmin", "true"));
        }

        return new ClaimsIdentity(claims, authenticationScheme);
    }
}
