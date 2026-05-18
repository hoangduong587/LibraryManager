using System.Net;
using System.Net.Http.Json;
using LibraryManager.Shared.Dtos;
public class BorrowHistoryService
{
    private readonly HttpClient _http;

    public BorrowHistoryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(bool Forbidden, List<BorrowHistoryDto> Data)> GetAllHistoryAsync()
    {
        var response = await _http.GetAsync("api/borrowhistory");

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            return (true, new List<BorrowHistoryDto>());

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<List<BorrowHistoryDto>>() ?? new();

        return (false, data);
    }

    public async Task<List<BorrowHistoryDto>> GetMyHistoryAsync()
    {
        return await _http.GetFromJsonAsync<List<BorrowHistoryDto>>("api/borrowhistory/my")
               ?? new();
    }
}
