using BookManagement.Models.Entities;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookManagement.Services.Repository
{
    public class BookService : IBookService
    {
        private readonly ProjectPrnContext _dbContext;

        public BookService()
        {
            _dbContext = new ProjectPrnContext();
            EnsureReaderReviewsTable();
        }

        private void EnsureReaderReviewsTable()
        {
            _dbContext.Database.ExecuteSqlRaw(@"
IF OBJECT_ID(N'[dbo].[ReaderReviews]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ReaderReviews]
    (
        [ReviewID] varchar(400) NOT NULL CONSTRAINT [PK_ReaderReviews] PRIMARY KEY,
        [ReaderID] varchar(400) NOT NULL,
        [BookID] varchar(400) NOT NULL,
        [Rating] int NOT NULL CONSTRAINT [CK_ReaderReviews_Rating] CHECK ([Rating] >= 1 AND [Rating] <= 5),
        [Comment] nvarchar(1000) NOT NULL,
        [CreatedAt] datetime NOT NULL CONSTRAINT [DF_ReaderReviews_CreatedAt] DEFAULT (getdate()),
        CONSTRAINT [FK_ReaderReviews_Reader] FOREIGN KEY ([ReaderID]) REFERENCES [dbo].[Reader]([ReaderID]),
        CONSTRAINT [FK_ReaderReviews_Books] FOREIGN KEY ([BookID]) REFERENCES [dbo].[Books]([BookID]),
        CONSTRAINT [UX_ReaderReviews_Reader_Book] UNIQUE ([ReaderID], [BookID])
    );
END");
        }

       
        private BookModel MapToModel(Book entity)
        {
            string authorName = entity.Author?.AuthorNavigation?.FullName ?? "Unknown Author";

            return new BookModel
            {
                Id = entity.BookId,
                Title = entity.Title,
                Author = authorName,
                Category = entity.Category,
                Price = (double)entity.Price,
                Status = entity.Status switch
                {
                    true => "Approved",
                    false => "Rejected",
                    null => "Pending"
                },
                Description = entity.Description,
                SubmittedDate = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsDeleted = entity.IsDeleted,
                CoverImagePath = "/Assets/Covers/placeholder.jpg",
                Rating = entity.ReaderReviews.Count == 0
                    ? 0
                    : Math.Round(entity.ReaderReviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero)
            };
        }

        public IEnumerable<BookModel> GetApprovedBooks()
        {
            var books = _dbContext.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Include(b => b.ReaderReviews)
                .Where(b => b.Status == true && !b.IsDeleted)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetPendingBooks()
        {
            var books = _dbContext.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Include(b => b.ReaderReviews)
                .Where(b => b.Status == null && !b.IsDeleted)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetMyBooks(string authorId)
        {
            var books = _dbContext.Books
                .AsNoTracking()
                .Include(b => b.Author )
                .ThenInclude(a => a.AuthorNavigation)
                .Include(b => b.ReaderReviews)
                .Where(b => b.AuthorId == authorId && !b.IsDeleted)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetAllBooks()
        {
            var books = _dbContext.Books.AsNoTracking().Where(b => !b.IsDeleted)
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Include(b => b.ReaderReviews)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public BookModel GetBookById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null!;

            var book = _dbContext.Books
        .AsNoTracking()
        .Include(b => b.Author)
        .ThenInclude(a => a.AuthorNavigation)
        .Include(b => b.ReaderReviews)
       .FirstOrDefault(b =>b.BookId == id && !b.IsDeleted);
            return book != null ? MapToModel(book) : null!;
           

        }

        public void CreateBook(BookModel bookModel)
        {
           

            string authorId = "c1916a5c-7dba-489d-b88c-a48d1738a765"; // seeded fallback Admin/Author
            
            var account = _dbContext.Accounts.FirstOrDefault(a => a.FullName==bookModel.Author);
            if (account != null)
            {
                authorId = account.AccountId;
                var isAuthor = _dbContext.Authors.Any(a => a.AuthorId == authorId);
                if (!isAuthor)
                {
                    _dbContext.Authors.Add(new Author { AuthorId = authorId });
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                var firstAuthor = _dbContext.Authors.FirstOrDefault();
                if (firstAuthor != null)
                {
                    authorId = firstAuthor.AuthorId;
                }
            }

            var book = new Book
            {
                BookId = Guid.NewGuid().ToString(),
                AuthorId = authorId,
                Title = bookModel.Title,
                Description = bookModel.Description,
                Category = bookModel.Category,
                Price = (decimal)bookModel.Price,
                
                Status = null,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            bookModel.Id = book.BookId;
        }

        public void UpdateBook(BookModel bookModel)
        {
            if (string.IsNullOrEmpty(bookModel.Id)) return;

            var existing = _dbContext.Books.FirstOrDefault(b => b.BookId == bookModel.Id);
            if (existing != null)
            {
                existing.Title = bookModel.Title;
                existing.Description = bookModel.Description;
                existing.Category = bookModel.Category;
                existing.Price = (decimal)bookModel.Price;

                // Reset Status to null (Pending) if it was Rejected (false) so Admin can re-evaluate the updated manuscript
                if (existing.Status == false)
                {
                    existing.Status = null;
                    bookModel.Status = "Pending";
                }

                _dbContext.Books.Update(existing);
                _dbContext.SaveChanges();
            }
        }

        public void ApproveBook(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return;

            var book = _dbContext.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book != null)
            {
                book.Status = true; // Approved
                _dbContext.Books.Update(book);
                _dbContext.SaveChanges();
            }
        }

        public void RejectBook(string bookId, string comment)
        {
            if (string.IsNullOrEmpty(bookId)) return;

            var book = _dbContext.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book != null)
            {
                book.Status = false; // Rejected
                _dbContext.Books.Update(book);
                _dbContext.SaveChanges();
            }
        }

        public void DeleteBook(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return;

            var book = _dbContext.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book != null)
            {
                book.IsDeleted = true;
                _dbContext.SaveChanges();
            }
        }
      
    }
}
