using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace LibraryManager.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return AnonymousState();

            JwtSecurityToken? jwt;

            try
            {
                jwt = _tokenHandler.ReadJwtToken(token);
            }
            catch
            {
                // Token is invalid → remove it
                await _localStorage.RemoveItemAsync("authToken");
                return AnonymousState();
            }

            // Token expired?
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync("authToken");
                return AnonymousState();
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }


        // ---------------------------------------------------------
        // LOGIN
        // ---------------------------------------------------------
        public void NotifyUserAuthentication(string token)
        {
            var jwt = _tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(user))
            );
        }

        // ---------------------------------------------------------
        // LOGOUT
        // ---------------------------------------------------------
        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(
                Task.FromResult(AnonymousState())
            );
        }

        // ---------------------------------------------------------
        // Helper
        // ---------------------------------------------------------
        private AuthenticationState AnonymousState()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
