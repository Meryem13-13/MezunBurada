using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace MezunBurada.Web.Data;

// Generates and verifies password-reset tokens. Only the SHA-256 hash of a token is ever
// persisted — the raw token exists only in the link sent to the user, so a database leak
// alone can't be used to reset anyone's password.
public static class PasswordResetTokenHelper
{
    public static (string RawToken, string Hash) Generate()
    {
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        return (rawToken, Hash(rawToken));
    }

    public static string Hash(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));

    public static bool Matches(string rawToken, string storedHash) =>
        CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(Hash(rawToken)),
            System.Text.Encoding.UTF8.GetBytes(storedHash));
}
