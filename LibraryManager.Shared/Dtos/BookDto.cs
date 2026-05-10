namespace LibraryManager.Shared.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public string? Author { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }

    // Borrower info (read-only to client)
    public int? BorrowerId { get; set; }
    public string? BorrowerUserName { get; set; }

    // Availability is still shown to the client
    public bool Availability { get; set; }
}
