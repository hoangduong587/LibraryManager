using AutoMapper;
using LibraryManager.Api.Data;
using LibraryManager.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // GET DASHBOARD SUMMARY
        // ---------------------------------------------------------
        [HttpGet("summary")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboardSummary()
        {
            var now = DateTime.UtcNow;
            var last7Days = now.AddDays(-7);

            var dto = new AdminDashboardDto
            {
                // Users
                TotalUsers = await _context.ProfileUsers.CountAsync(),
                ActiveBorrowers = await _context.ProfileUsers
                    .Where(p => p.BorrowedBooks.Any())
                    .CountAsync(),

                // Books
                TotalBooks = await _context.BooksList.CountAsync(),
                AvailableBooks = await _context.BooksList
                    .Where(b => b.Availability == true)
                    .CountAsync(),
                BorrowedBooks = await _context.BooksList
                    .Where(b => b.Availability == false)
                    .CountAsync(),

                // History
                TotalBorrowEvents = await _context.BorrowHistories
                    .Where(h => h.BorrowDate != null)
                    .CountAsync(),
                TotalReturnEvents = await _context.BorrowHistories
                    .Where(h => h.ReturnDate != null)
                    .CountAsync(),

                // Trending (last 7 days)
                BorrowedLast7Days = await _context.BorrowHistories
                    .Where(h => h.BorrowDate >= last7Days)
                    .CountAsync(),
                ReturnedLast7Days = await _context.BorrowHistories
                    .Where(h => h.ReturnDate >= last7Days)
                    .CountAsync()
            };

            return Ok(dto);
        }
    }
}
