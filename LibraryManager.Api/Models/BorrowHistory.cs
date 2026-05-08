using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Api.Models
{
    public class BorrowHistory
    {
        [Key]
        public int Id { get; set; }

        // FK → ProfileUser (who performed the action)
        [Required]
        public int ProfileUserId { get; set; }
        
        [ForeignKey(nameof(ProfileUserId))]
        public ProfileUser ProfileUser { get; set; }

        // FK → BooksList (which physical book)
        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public BooksList Book { get; set; }

        // Nullable because this row may represent a return event only
        public DateTime? BorrowDate { get; set; }

        // Nullable because the book may not be returned yet
        public DateTime? ReturnDate { get; set; }
    }
}
