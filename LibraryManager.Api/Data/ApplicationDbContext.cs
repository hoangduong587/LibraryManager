using LibraryManager.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Api.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProfileUser> ProfileUsers { get; set; }

        // FIX: Match the controller name "BooksList"
        public DbSet<BooksList> BooksList { get; set; }

        public DbSet<BorrowHistory> BorrowHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new ApplicationRole { Id = "2", Name = "Manager", NormalizedName = "MANAGER" },
                new ApplicationRole { Id = "3", Name = "Staff", NormalizedName = "STAFF" },
                new ApplicationRole { Id = "4", Name = "User", NormalizedName = "USER" }
            );
        }
    }
}
