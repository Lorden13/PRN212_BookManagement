using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IBookService
    {
        IEnumerable<BookModel> GetApprovedBooks();
        IEnumerable<BookModel> GetPendingBooks();
        IEnumerable<BookModel> GetMyBooks(int authorId);
        BookModel GetBookById(int id);
        void CreateBook(BookModel book);
        void UpdateBook(BookModel book);
        void ApproveBook(int bookId);
        void RejectBook(int bookId, string comment);
    }
}
