using Microsoft.AspNetCore.Identity;

namespace LibraryManager.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add any custom fields you want to store for each user
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Example: track when the user registered
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
