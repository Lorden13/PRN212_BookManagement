using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IBookService
    {
        IEnumerable<BookModel> GetApprovedBooks();
        IEnumerable<BookModel> GetPendingBooks();
        IEnumerable<BookModel> GetMyBooks(string authorId);
        BookModel GetBookById(string id);
        void CreateBook(BookModel book);
        void UpdateBook(BookModel book);
        void ApproveBook(string bookId);
        void RejectBook(string bookId, string comment);
        void DeleteBook(string bookId);
    }
}
