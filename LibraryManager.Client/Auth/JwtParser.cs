using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibraryManager.Client.Auth
{
    public static class JwtParser
    {
        private static readonly JwtSecurityTokenHandler _handler = new();

        public static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Enumerable.Empty<Claim>();

            JwtSecurityToken? jwt;

            try
            {
                jwt = _handler.ReadJwtToken(token);
            }
            catch
            {
                // Token is invalid or corrupted
                return Enumerable.Empty<Claim>();
            }

            // Optional: ignore expired tokens
            if (jwt.ValidTo < DateTime.UtcNow)
                return Enumerable.Empty<Claim>();

            return jwt.Claims;
        }
    }
}
