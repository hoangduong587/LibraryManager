using System.Net;
using System.Net.Http.Json;
using LibraryManager.Shared.Dtos;

public class ProfileUserService
{
    private readonly HttpClient _http;

    public ProfileUserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProfileUserDto> GetProfileAsync()
    {
        var response = await _http.GetAsync("api/profileuser");

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new Exception("You must be logged in.");

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        var profile = await response.Content.ReadFromJsonAsync<ProfileUserDto>();
        return profile ?? throw new Exception("Failed to deserialize profile user.");
    }

    public async Task<string> UpdateProfileAsync(UpdateProfileUserDto dto)
    {
        var response = await _http.PutAsJsonAsync("api/profileuser", dto);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "__UNAUTHORIZED__";

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return "Profile updated successfully.";
    }
}
