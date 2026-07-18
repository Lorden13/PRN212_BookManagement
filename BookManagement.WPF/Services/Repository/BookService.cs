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
                FilePath = entity.FilePath,
                CoverImagePath = "/Assets/Covers/placeholder.jpg"
            };
        }

        public IEnumerable<BookModel> GetApprovedBooks()
        {
            var books = _dbContext.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Where(b => b.Status == true)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetPendingBooks()
        {
            var books = _dbContext.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Where(b => b.Status == null)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetMyBooks(string authorId)
        {
            var books = _dbContext.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .Where(b => b.AuthorId == authorId)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetAllBooks()
        {
            var books = _dbContext.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .ToList();
            return books.Select(MapToModel).ToList();
        }

        public BookModel GetBookById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null!;

            var book = _dbContext.Books
                .Include(b => b.Author)
                .ThenInclude(a => a.AuthorNavigation)
                .FirstOrDefault(b => b.BookId == id);
            return book != null ? MapToModel(book) : null!;
        }

        public void CreateBook(BookModel bookModel)
        {
            string relativeFilePath = "Manuscripts/default_manuscript.pdf";
            //if (!string.IsNullOrEmpty(bookModel.FilePath) && File.Exists(bookModel.FilePath))
            //{
            //    relativeFilePath = FileStorageHelper.CopyPdfToStorage(bookModel.FilePath, bookModel.Title);   //error here
            //}

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
                FilePath = relativeFilePath,
                Status = null, // Pending by default
                CreatedAt = DateTime.Now
            };

            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            bookModel.Id = book.BookId;
            bookModel.FilePath = relativeFilePath;
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
                // Optionally delete physical file
                //if (!string.IsNullOrEmpty(book.FilePath))
                //{
                //    FileStorageHelper.DeletePdfFromStorage(book.FilePath); //error here
                //}

                _dbContext.Books.Remove(book);
                _dbContext.SaveChanges();
            }
        }
    }
}
