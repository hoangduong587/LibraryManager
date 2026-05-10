using LibraryManager.Api.Data;
using LibraryManager.Shared.Dtos;
using LibraryManager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class ProfileUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // GET /api/profileuser
        // Admin, Manager, Staff, or the user themself
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<ProfileUserDto>> GetProfile()
        {
            var userId = User.FindFirst("id")?.Value;

            if (userId == null)
                return Unauthorized("Invalid token.");

            var profile = await _context.ProfileUsers
                .Include(p => p.User)
                .Include(p => p.BorrowedBooks)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found.");

            // Build DTO
            var dto = new ProfileUserDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                UserName = profile.User.UserName,
                FirstName = profile.User.FirstName,
                LastName = profile.User.LastName,
                Email = profile.User.Email,
                BorrowedBooks = profile.BorrowedBooks.Select(b => new BorrowedBookDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    PublishYear = b.PublishYear
                }).ToList()
            };

            return Ok(dto);
        }

        // ---------------------------------------------------------
        // PUT /api/profileuser
        // Admin, Manager, or the user themself
        // Staff cannot update profiles
        // ---------------------------------------------------------
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileUserDto dto)
        {
            var userId = User.FindFirst("id")?.Value;

            if (userId == null)
                return Unauthorized("Invalid token.");

            var profile = await _context.ProfileUsers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found.");

            // Role check
            bool isAdmin = User.IsInRole("Admin");
            bool isManager = User.IsInRole("Manager");

            // Staff cannot update profiles
            if (!isAdmin && !isManager)
            {
                // Only the user themself can update their own profile
                if (profile.UserId != userId)
                    return Forbid("You are not allowed to update this profile.");
            }

            // Update ApplicationUser fields
            profile.User.FirstName = dto.FirstName;
            profile.User.LastName = dto.LastName;
            profile.User.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }
    }
}
