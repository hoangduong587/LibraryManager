using System.Net.Http.Json;
using LibraryManager.Shared.Dtos.Auth;

namespace LibraryManager.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    // ----------------------------------------------------
    // REGISTER
    // ----------------------------------------------------
    public async Task<AuthResponseDto?> Register(RegisterDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    // ----------------------------------------------------
    // LOGIN
    // ----------------------------------------------------
    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    // ----------------------------------------------------
    // LOGOUT
    // ----------------------------------------------------
    public async Task Logout()
    {
        try
        {
            // Optional: only works if your API has a logout endpoint
            await _http.PostAsync("api/auth/logout", null);
        }
        catch
        {
            // Ignore errors — client-side logout should always succeed
        }
    }
}
