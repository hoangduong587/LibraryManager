namespace LibraryManager.Shared.Dtos;

public class BorrowedBookDto
{
    public int Id { get; set; }                 // Unique physical book ID
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public int? PublishYear { get; set; }
}
