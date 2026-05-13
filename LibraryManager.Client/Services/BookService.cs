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

    public async Task<bool> CreateBookAsync(CreateBookDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/book", dto);
        return response.IsSuccessStatusCode;
    }


    public async Task<bool> UpdateBookAsync(int id, UpdateBookDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Book/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<BookDto>($"api/book/{id}");
        }
        catch
        {
            return null;
        }
    }

}
