namespace LibraryManager.Shared.Dtos;

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
