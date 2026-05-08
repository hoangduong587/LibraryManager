public class ProfileUserDto
{
    public int Id { get; set; }                 // ProfileUser Id
    public string UserId { get; set; } = "";    // ApplicationUser Id

    // ApplicationUser info
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";

    // Borrowed books
    public List<BorrowedBookDto> BorrowedBooks { get; set; } = new();
}

public class BorrowedBookDto
{
    public int BookId { get; set; }             // Unique physical book ID
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public int? PublishYear { get; set; }
}
