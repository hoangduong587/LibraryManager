using LibraryManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Api.Data
{
    public class BookSeeder
    {
        private readonly ApplicationDbContext _context;

        public BookSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedBooksAsync()
        {
            if (await _context.BooksList.AnyAsync())
                return; // Already seeded

            var books = new List<BooksList>();

            int totalBooks = 100;
            int totalGenres = 25;   // each genre repeats 4 times
            int totalAuthors = 50;  // each author repeats 2 times

            int startYear = 1980;
            int endYear = 2000;
            int yearRange = endYear - startYear + 1;

            for (int i = 1; i <= totalBooks; i++)
            {
                // Genre repeats every 4 books
                int genreIndex = ((i - 1) / 4) + 1; // 1–25
                if (genreIndex > totalGenres)
                    genreIndex = totalGenres;

                // Author repeats every 2 books
                int authorIndex = ((i - 1) / 2) + 1; // 1–50
                if (authorIndex > totalAuthors)
                    authorIndex = totalAuthors;

                // Publish year cycles evenly
                int year = startYear + ((i - 1) % yearRange);

                books.Add(new BooksList
                {
                    Title = $"Book{i}",
                    Genre = $"Genre{genreIndex}",
                    Author = $"Author{authorIndex}",
                    PublishYear = year,
                    Description = $"Description for Book{i}",
                    Availability = true,
                    Location = $"Location{genreIndex}",

                    ProfileUserId = null // all books available
                });
            }

            await _context.BooksList.AddRangeAsync(books);
            await _context.SaveChangesAsync();
        }
    }
}
