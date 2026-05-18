using LibraryManager.Shared.Dtos;
using System.Net.Http.Json;

public class ProfileService
{
    private readonly HttpClient _http;

    public ProfileService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProfileUserDto?> GetProfileAsync()
    {
        return await _http.GetFromJsonAsync<ProfileUserDto>("api/profileuser");
    }

    public async Task<HttpResponseMessage> UpdateProfileAsync(UpdateProfileUserDto dto)
    {
        return await _http.PutAsJsonAsync("api/profileuser", dto);
    }
    public async Task<ProfileUserDto?> GetProfileByUserIdAsync(string userId)
    {
        return await _http.GetFromJsonAsync<ProfileUserDto>($"api/profileuser/{userId}");
    }

    public async Task<HttpResponseMessage> UpdateUserProfileAsync(string userId, UpdateProfileUserDto dto)
    {
        return await _http.PutAsJsonAsync($"api/profileuser/{userId}", dto);
    }

}
