using LibraryManager.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManager.Api.Models;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin,Manager")]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // ---------------------------------------------------------
    // GET: /api/admin/users
    // Admin + Manager can view all users
    // ---------------------------------------------------------
    [HttpGet("all")]
    public async Task<ActionResult<List<AdminUserDto>>> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new AdminUserDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Roles = roles.ToList()
            });
        }

        return Ok(result);
    }
    [HttpPut("update/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] AdminUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        // Identity requires normalized fields
        user.NormalizedUserName = dto.UserName.ToUpper();
        user.NormalizedEmail = dto.Email.ToUpper();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }



}
