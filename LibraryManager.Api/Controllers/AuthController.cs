using LibraryManager.Shared.Dtos.Auth;
using LibraryManager.Api.Helpers;
using LibraryManager.Api.Models;
using LibraryManager.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManager.Api.Data;

namespace LibraryManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            JwtService jwtService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _context = context;
        }

        // -------------------------
        // REGISTER (Auto Login)
        // -------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Validate FirstName
            if (!ValidationInputHelper.IsValidName(dto.FirstName))
                return BadRequest("First name must contain only letters and numbers and cannot be blank.");

            // Validate LastName
            if (!ValidationInputHelper.IsValidName(dto.LastName))
                return BadRequest("Last name must contain only letters and numbers and cannot be blank.");

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role = User
            if (!await _roleManager.RoleExistsAsync("User"))
                return StatusCode(500, "User role does not exist in the system.");

            await _userManager.AddToRoleAsync(user, "User");

            // Automatically create ProfileUser
            var profile = new ProfileUser
            {
                UserId = user.Id
            };

            _context.ProfileUsers.Add(profile);
            await _context.SaveChangesAsync();

            // Auto-login: generate JWT token
            var token = await _jwtService.GenerateToken(user);

            return Ok(new
            {
                Message = "Registration successful. Logged in automatically.",
                Token = token
            });
        }

        // -------------------------
        // LOGIN
        // -------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            // Check password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid email or password.");

            // Generate JWT
            var token = await _jwtService.GenerateToken(user);

            return Ok(new
            {
                Message = "Login successful.",
                Token = token
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new
            {
                message = "Logout successful. Please remove the token on the client."
            });
        }
    }
}
