using LibraryManager.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LibraryManager.Api.Data
{
    public class DataSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;   // ⭐ Added DbContext

        public DataSeeder(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            ApplicationDbContext context)                 // ⭐ Inject DbContext
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
            _context = context;                           // ⭐ Assign DbContext
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedManagerAccountsAsync();
            await SeedStaffAccountsAsync();
            await SeedUserAccountsAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Manager", "Staff", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var appRole = new ApplicationRole { Name = role };
                    await _roleManager.CreateAsync(appRole);
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminSection = _config.GetSection("AdminUser");

            string email = adminSection["Email"];
            string username = adminSection["UserName"];
            string password = adminSection["Password"];
            string firstName = adminSection["FirstName"];
            string lastName = adminSection["LastName"];

            var existingAdmin = await _userManager.FindByEmailAsync(email);
            if (existingAdmin != null)
                return;

            var adminUser = new ApplicationUser
            {
                Email = email,
                UserName = username,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");

                // ⭐ Create ProfileUser
                _context.ProfileUsers.Add(new ProfileUser
                {
                    UserId = adminUser.Id
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedManagerAccountsAsync()
        {
            int count = _config.GetValue<int>("SeedSettings:ManagerCount");
            string defaultPassword = _config.GetValue<string>("SeedSettings:DefaultPassword");

            for (int i = 1; i <= count; i++)
            {
                string email = $"manager{i}@system.com";
                string username = $"manager{i}";

                if (await _userManager.FindByEmailAsync(email) != null)
                    continue;

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = username,
                    FirstName = $"Manager{i}",
                    LastName = "Account",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Manager");

                    // ⭐ Create ProfileUser
                    _context.ProfileUsers.Add(new ProfileUser
                    {
                        UserId = user.Id
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task SeedStaffAccountsAsync()
        {
            int count = _config.GetValue<int>("SeedSettings:StaffCount");
            string defaultPassword = _config.GetValue<string>("SeedSettings:DefaultPassword");

            for (int i = 1; i <= count; i++)
            {
                string email = $"staff{i}@system.com";
                string username = $"staff{i}";

                if (await _userManager.FindByEmailAsync(email) != null)
                    continue;

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = username,
                    FirstName = $"Staff{i}",
                    LastName = "Account",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Staff");

                    // ⭐ Create ProfileUser
                    _context.ProfileUsers.Add(new ProfileUser
                    {
                        UserId = user.Id
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task SeedUserAccountsAsync()
        {
            int count = _config.GetValue<int>("SeedSettings:UserCount");
            string defaultPassword = _config.GetValue<string>("SeedSettings:DefaultPassword");

            for (int i = 1; i <= count; i++)
            {
                string email = $"user{i}@system.com";
                string username = $"user{i}";

                if (await _userManager.FindByEmailAsync(email) != null)
                    continue;

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = username,
                    FirstName = $"User{i}",
                    LastName = "Account",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    // ⭐ Create ProfileUser
                    _context.ProfileUsers.Add(new ProfileUser
                    {
                        UserId = user.Id
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
