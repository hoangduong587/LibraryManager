public class BorrowHistoryDto
{
    public int Id { get; set; }

    // Borrower info
    public int ProfileUserId { get; set; }
    public string? BorrowerUserName { get; set; }

    // Book info
    public int BookId { get; set; }
    public string? BookTitle { get; set; }

    // Event timestamps
    public DateTime? BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
