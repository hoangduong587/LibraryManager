using System.Net.Http.Json;
using LibraryManager.Shared.Dtos;

public class AdminUserService
{
    private readonly HttpClient _http;

    public AdminUserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AdminUserDto>?> GetAllUsersAsync()
    {
        return await _http.GetFromJsonAsync<List<AdminUserDto>>("api/admin/all");
    }
    

}
