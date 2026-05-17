using System.Net;
using System.Net.Http.Json;

public class BorrowService
{
    private readonly HttpClient _http;

    public BorrowService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> BorrowBookAsync(int bookId)
    {
        var response = await _http.PostAsync($"api/Book/{bookId}/borrow", null);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "__UNAUTHORIZED__";

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return "Book borrowed successfully.";
    }

    public async Task<string> ReturnBookAsync(int bookId)
    {
        var response = await _http.PostAsync($"api/Book/{bookId}/return", null);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "__UNAUTHORIZED__";

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return "Book returned successfully.";
    }
}
