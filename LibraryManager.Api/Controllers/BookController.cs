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
   // [Authorize]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ---------------------------------------------------------
        // GET ALL BOOKS
        // Admin, Manager, Staff, User
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.BooksList.ToListAsync();
            return Ok(_mapper.Map<List<BookDto>>(books));
        }

        // ---------------------------------------------------------
        // GET BOOK BY ID
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _context.BooksList.FindAsync(id);

            if (book == null)
                return NotFound("Book not found.");

            return Ok(_mapper.Map<BookDto>(book));
        }

        // ---------------------------------------------------------
        // CREATE BOOK
        // Admin, Manager only
        // ---------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto dto)
        {
            var book = _mapper.Map<BooksList>(dto);

            _context.BooksList.Add(book);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<BookDto>(book));
        }

        // ---------------------------------------------------------
        // UPDATE BOOK
        // Admin, Manager only
        // ---------------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto dto)
        {
            var book = await _context.BooksList.FindAsync(id);

            if (book == null)
                return NotFound("Book not found.");

            // AutoMapper updates only non-null fields
            _mapper.Map(dto, book);

            await _context.SaveChangesAsync();

            return Ok("Book updated successfully.");
        }

        // ---------------------------------------------------------
        // DELETE BOOK
        // Admin only
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.BooksList.FindAsync(id);

            if (book == null)
                return NotFound("Book not found.");

            _context.BooksList.Remove(book);
            await _context.SaveChangesAsync();

            return Ok("Book deleted successfully.");
        }

        // ---------------------------------------------------------
        // BORROW BOOK
        // User borrows a book → adds to ProfileUser.BorrowedBooks
        // ---------------------------------------------------------
        [HttpPost("{bookId}/borrow")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.FindFirst("id")?.Value;

            var profile = await _context.ProfileUsers
                .Include(p => p.BorrowedBooks)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found.");

            var book = await _context.BooksList.FindAsync(bookId);

            if (book == null)
                return NotFound("Book not found.");

            if (!book.Availability)
                return BadRequest("Book is already borrowed.");

            // Borrow logic
            book.Availability = false;
            profile.BorrowedBooks.Add(book);

            // Create BorrowHistory entry
            var history = new BorrowHistory
            {
                ProfileUserId = profile.Id,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow,
                ReturnDate = null
            };

            _context.BorrowHistories.Add(history);

            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully.");
        }

        // ---------------------------------------------------------
        // RETURN BOOK
        // User returns a book → removes from BorrowedBooks
        // ---------------------------------------------------------
        [HttpPost("{bookId}/return")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var userId = User.FindFirst("id")?.Value;

            var profile = await _context.ProfileUsers
                .Include(p => p.BorrowedBooks)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profile not found.");

            var book = profile.BorrowedBooks.FirstOrDefault(b => b.BookId == bookId);

            if (book == null)
                return BadRequest("You did not borrow this book.");

            // Return logic
            book.Availability = true;
            profile.BorrowedBooks.Remove(book);

            // Create BorrowHistory entry
            var history = new BorrowHistory
            {
                ProfileUserId = profile.Id,
                BookId = bookId,
                BorrowDate = null, // return-only event
                ReturnDate = DateTime.UtcNow
            };

            _context.BorrowHistories.Add(history);

            await _context.SaveChangesAsync();

            return Ok("Book returned successfully.");
        }

    }
}
