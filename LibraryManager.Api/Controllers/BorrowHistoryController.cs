using AutoMapper;
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
    [Authorize]
    public class BorrowHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BorrowHistoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ---------------------------------------------------------
        // GET ALL HISTORY (Admin, Manager, Staff)
        // ---------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<ActionResult<IEnumerable<BorrowHistoryDto>>> GetAllHistory()
        {
            var history = await _context.BorrowHistories
                .Include(h => h.ProfileUser)
                    .ThenInclude(p => p.User)
                .Include(h => h.Book)
                .OrderByDescending(h => h.Id)
                .ToListAsync();

            return Ok(_mapper.Map<List<BorrowHistoryDto>>(history));
        }

        // ---------------------------------------------------------
        // GET HISTORY FOR CURRENT USER
        // ---------------------------------------------------------
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<BorrowHistoryDto>>> GetMyHistory()
        {
            var userId = User.FindFirst("id")?.Value;

            var profile = await _context.ProfileUsers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found.");

            var history = await _context.BorrowHistories
                .Where(h => h.ProfileUserId == profile.Id)
                .Include(h => h.Book)
                .Include(h => h.ProfileUser)
                    .ThenInclude(p => p.User)
                .OrderByDescending(h => h.Id)
                .ToListAsync();

            return Ok(_mapper.Map<List<BorrowHistoryDto>>(history));
        }

        // ---------------------------------------------------------
        // GET HISTORY BY BOOK ID (Admin, Manager, Staff)
        // ---------------------------------------------------------
        [HttpGet("book/{bookId}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<ActionResult<IEnumerable<BorrowHistoryDto>>> GetHistoryByBook(int bookId)
        {
            var history = await _context.BorrowHistories
                .Where(h => h.BookId == bookId)
                .Include(h => h.ProfileUser)
                    .ThenInclude(p => p.User)
                .Include(h => h.Book)
                .OrderByDescending(h => h.Id)
                .ToListAsync();

            return Ok(_mapper.Map<List<BorrowHistoryDto>>(history));
        }
    }
}
