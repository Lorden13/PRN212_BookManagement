using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagement.Services.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ProjectPrnContext _context;

        public BookRepository()
        {
            _context = new ProjectPrnContext();
        }

        public async Task<Book?> GetByIdAsync(string bookId)
        {
            return await _context.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByAuthorIdAsync(string authorId)
        {
            return await _context.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
