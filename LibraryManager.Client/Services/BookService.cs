using System.Net.Http.Json;
using LibraryManager.Shared.Dtos;
namespace LibraryManager.Client.Services;

public class BookService : IBookService
{
    private readonly HttpClient _http;

    public BookService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        var result = await _http.GetFromJsonAsync<List<BookDto>>("api/Book");
        return result ?? new List<BookDto>();
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Book", dto);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<BookDto>();
        return created!;
    }
}
