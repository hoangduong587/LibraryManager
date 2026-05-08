using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Api.Dtos.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
