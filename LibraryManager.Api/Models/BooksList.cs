using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Api.Models
{
    public class BooksList
    {
        [Key]
        public int BookId { get; set; }

        // Nullable FK → ProfileUser (who borrowed the book, if any)
        public int? ProfileUserId { get; set; }

        [ForeignKey(nameof(ProfileUserId))]
        public ProfileUser? ProfileUser { get; set; }

        // Book details
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Genre { get; set; }

        [MaxLength(150)]
        public string? Author { get; set; }

        public int? PublishYear { get; set; }

        public string? Description { get; set; }

        // Availability: true = available, false = borrowed
        public bool Availability { get; set; } = true;

        // Physical location in the library
        [MaxLength(100)]
        public string? Location { get; set; }
    }
}
