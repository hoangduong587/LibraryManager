using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Api.Models
{
    public class ProfileUser
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key → ApplicationUser
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // Navigation: One ProfileUser can borrow many books
        public ICollection<BooksList> BorrowedBooks { get; set; } = new List<BooksList>();
    }
}
