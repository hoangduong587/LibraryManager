using LibraryManager.Shared.Dtos;

namespace LibraryManager.Client.Services;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<bool> CreateBookAsync(CreateBookDto dto);
    Task<bool> UpdateBookAsync(int id, UpdateBookDto dto);

}
