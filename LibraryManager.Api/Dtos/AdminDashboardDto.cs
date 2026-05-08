public class AdminDashboardDto
{
    // User stats
    public int TotalUsers { get; set; }
    public int ActiveBorrowers { get; set; }

    // Book stats
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }

    // History stats
    public int TotalBorrowEvents { get; set; }
    public int TotalReturnEvents { get; set; }

    // Optional: trending metrics
    public int BorrowedLast7Days { get; set; }
    public int ReturnedLast7Days { get; set; }
}
