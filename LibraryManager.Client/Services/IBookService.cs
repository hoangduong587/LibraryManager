using LibraryManager.Shared.Dtos;

namespace LibraryManager.Client.Services;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<BookDto> CreateBookAsync(CreateBookDto dto);

}
