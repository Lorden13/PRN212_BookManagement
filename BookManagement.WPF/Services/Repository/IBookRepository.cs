using BookManagement.WPF.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagement.Services.Repository
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(string bookId);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetByAuthorIdAsync(string authorId);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Book book);
    }
}
