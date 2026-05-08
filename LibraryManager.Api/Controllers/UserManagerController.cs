using LibraryManager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // -------------------------------------------------------
        // GET ALL USERS
        // -------------------------------------------------------
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // -------------------------------------------------------
        // CHANGE ROLE
        // -------------------------------------------------------
        [HttpPut("role/{userId}")]
        public async Task<IActionResult> ChangeRole(string userId, [FromQuery] string newRole)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);

            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
                return NotFound("User not found.");

            var targetRoles = await _userManager.GetRolesAsync(targetUser);
            var targetRole = targetRoles.FirstOrDefault() ?? "User";

            bool isAdmin = currentRoles.Contains("Admin");
            bool isManager = currentRoles.Contains("Manager");
            bool isStaff = currentRoles.Contains("Staff");

            // -------------------------------------------------------
            // PREVENT SELF-ROLE CHANGE
            // -------------------------------------------------------
            if (currentUser.Id == targetUser.Id)
                return Forbid("You cannot change your own role.");

            // -------------------------------------------------------
            // STAFF RULES
            // -------------------------------------------------------
            if (isStaff)
                return Forbid("Staff cannot change roles.");

            // -------------------------------------------------------
            // MANAGER RULES
            // -------------------------------------------------------
            if (isManager)
            {
                if (targetRole == "Admin")
                    return Forbid("Managers cannot modify Admin accounts.");

                if (newRole == "Admin")
                    return Forbid("Managers cannot promote anyone to Admin.");

                if (newRole == "Manager")
                    return Forbid("Managers cannot promote anyone to Manager.");

                if (!(targetRole == "User" || targetRole == "Staff"))
                    return Forbid("Managers can only modify Staff or User accounts.");
            }

            // -------------------------------------------------------
            // ADMIN RULES
            // -------------------------------------------------------
            if (isAdmin)
            {
                // Admin cannot demote themselves
                if (targetUser.Id == currentUser.Id)
                    return Forbid("Admin cannot change their own role.");
            }

            // -------------------------------------------------------
            // APPLY ROLE CHANGE
            // -------------------------------------------------------
            await _userManager.RemoveFromRolesAsync(targetUser, targetRoles);
            await _userManager.AddToRoleAsync(targetUser, newRole);

            return Ok($"User role updated to {newRole}.");
        }

        // -------------------------------------------------------
        // DELETE USER
        // -------------------------------------------------------
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);

            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
                return NotFound("User not found.");

            var targetRoles = await _userManager.GetRolesAsync(targetUser);
            var targetRole = targetRoles.FirstOrDefault() ?? "User";

            bool isAdmin = currentRoles.Contains("Admin");
            bool isManager = currentRoles.Contains("Manager");
            bool isStaff = currentRoles.Contains("Staff");

            // -------------------------------------------------------
            // PREVENT SELF-DELETE
            // -------------------------------------------------------
            if (currentUser.Id == targetUser.Id)
                return Forbid("You cannot delete your own account.");

            // -------------------------------------------------------
            // STAFF RULES
            // -------------------------------------------------------
            if (isStaff)
            {
                if (targetRole != "User")
                    return Forbid("Staff can only delete User accounts.");
            }

            // -------------------------------------------------------
            // MANAGER RULES
            // -------------------------------------------------------
            if (isManager)
            {
                if (targetRole == "Admin")
                    return Forbid("Managers cannot delete Admin accounts.");

                if (targetRole == "Manager")
                    return Forbid("Managers cannot delete other Managers.");
            }

            // -------------------------------------------------------
            // ADMIN RULES
            // -------------------------------------------------------
            if (isAdmin)
            {
                if (targetUser.Id == currentUser.Id)
                    return Forbid("Admin cannot delete themselves.");
            }

            await _userManager.DeleteAsync(targetUser);

            return Ok("User deleted successfully.");
        }
    }
}
