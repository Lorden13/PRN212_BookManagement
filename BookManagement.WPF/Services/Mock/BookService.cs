using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.Services.Utils;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagement.Services.Mock
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IApprovalService _approvalService;

        // In-memory mapping of stable polynomial hash int IDs to SQL GUID strings
        private static readonly Dictionary<int, string> IdMap = new Dictionary<int, string>();
        private static readonly Dictionary<string, int> ReverseIdMap = new Dictionary<string, int>();

        public BookService()
        {
            _bookRepository = new BookRepository();
            _approvalService = new ApprovalService();
            
            // Warm up ID mappings
            var books = Task.Run(async () => await _bookRepository.GetAllAsync()).Result;
            foreach (var book in books)
            {
                GetOrCreateIntId(book.BookId);
            }
        }

        private static int GetOrCreateIntId(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return 0;
            if (ReverseIdMap.TryGetValue(guid, out int existing)) return existing;

            // FNV-1a deterministic hash
            uint hash = 2166136261;
            foreach (char c in guid)
            {
                hash = (hash ^ c) * 16777619;
            }
            int intId = (int)(hash & 0x7FFFFFFF); // positive integer

            // Handle collision (increment until unique)
            while (IdMap.ContainsKey(intId) && IdMap[intId] != guid)
            {
                intId++;
            }

            IdMap[intId] = guid;
            ReverseIdMap[guid] = intId;
            return intId;
        }

        private static string? GetGuidFromInt(int intId)
        {
            return IdMap.TryGetValue(intId, out string? guid) ? guid : null;
        }

        private BookModel MapToModel(Book entity)
        {
            int intId = GetOrCreateIntId(entity.BookId);
            string authorName = entity.Author?.AuthorNavigation?.FullName ?? "Alice Johnson";

            return new BookModel
            {
                Id = intId,
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
                CoverImagePath = "/Assets/Covers/placeholder.jpg" // static cover placeholder
            };
        }

        public IEnumerable<BookModel> GetApprovedBooks()
        {
            var books = Task.Run(async () => await _bookRepository.GetAllAsync()).Result;
            return books.Where(b => b.Status == true).Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetPendingBooks()
        {
            var books = Task.Run(async () => await _bookRepository.GetAllAsync()).Result;
            return books.Where(b => b.Status == null).Select(MapToModel).ToList();
        }

        public IEnumerable<BookModel> GetMyBooks(int authorId)
        {
            // Returns all books from DB, UI ViewModels will filter locally by Author's name (e.g. Alice Johnson)
            var books = Task.Run(async () => await _bookRepository.GetAllAsync()).Result;
            return books.Select(MapToModel).ToList();
        }

        public BookModel GetBookById(int id)
        {
            string? guid = GetGuidFromInt(id);
            if (string.IsNullOrEmpty(guid)) return null!;

            var book = Task.Run(async () => await _bookRepository.GetByIdAsync(guid)).Result;
            return book != null ? MapToModel(book) : null!;
        }

        public void CreateBook(BookModel bookModel)
        {
            try
            {
                // 1. Handle file storage
                string relativeFilePath = "Manuscripts/default_manuscript.pdf";
                if (!string.IsNullOrEmpty(bookModel.FilePath) && File.Exists(bookModel.FilePath))
                {
                    relativeFilePath = FileStorageHelper.CopyPdfToStorage(bookModel.FilePath, bookModel.Title);
                }

                // 2. Resolve AuthorId
                string authorId = "c1916a5c-7dba-489d-b88c-a48d1738a765"; // Default fallback (seeded Admin AccountID)
                
                using (var dbContext = new ProjectPrnContext())
                {
                    var account = dbContext.Accounts.FirstOrDefault(a => a.FullName.ToLower() == bookModel.Author.ToLower());
                    if (account != null)
                    {
                        authorId = account.AccountId;
                        // Ensure this Account is registered as an Author in the Author table to prevent FK constraint error
                        var isAuthor = dbContext.Authors.Any(a => a.AuthorId == authorId);
                        if (!isAuthor)
                        {
                            dbContext.Authors.Add(new Author { AuthorId = authorId });
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        // Fallback: check if there's any author in the DB
                        var firstAuthor = dbContext.Authors.FirstOrDefault();
                        if (firstAuthor != null)
                        {
                            authorId = firstAuthor.AuthorId;
                        }
                        else
                        {
                            // Absolute fallback: register the default account as an Author
                            var fallbackAccount = dbContext.Accounts.FirstOrDefault();
                            if (fallbackAccount != null)
                            {
                                authorId = fallbackAccount.AccountId;
                                var isAuthor = dbContext.Authors.Any(a => a.AuthorId == authorId);
                                if (!isAuthor)
                                {
                                    dbContext.Authors.Add(new Author { AuthorId = authorId });
                                    dbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Không tìm thấy tài khoản nào trong hệ thống để gán tác giả cho sách.");
                            }
                        }
                    }
                }

                // 3. Construct entity
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

                Task.Run(async () => await _bookRepository.AddAsync(book)).Wait();
                
                // Map the newly created GUID to the bookModel's newly generated int ID
                int intId = GetOrCreateIntId(book.BookId);
                bookModel.Id = intId;
                bookModel.FilePath = relativeFilePath;
            }
            catch (Exception ex)
            {
                var baseEx = ex.InnerException ?? ex;
                var innerMsg = baseEx.InnerException?.Message ?? baseEx.Message;
                throw new Exception($"Không thể tạo sách. Chi tiết lỗi CSDL: {innerMsg}", baseEx);
            }
        }

        public void UpdateBook(BookModel bookModel)
        {
            try
            {
                string? guid = GetGuidFromInt(bookModel.Id);
                if (string.IsNullOrEmpty(guid)) return;

                var existing = Task.Run(async () => await _bookRepository.GetByIdAsync(guid)).Result;
                if (existing != null)
                {
                    // Delete old PDF file if a new one is selected
                    if (!string.IsNullOrEmpty(bookModel.FilePath) && File.Exists(bookModel.FilePath) && bookModel.FilePath != existing.FilePath)
                    {
                        FileStorageHelper.DeletePdfFromStorage(existing.FilePath);
                        string newPath = FileStorageHelper.CopyPdfToStorage(bookModel.FilePath, bookModel.Title);
                        existing.FilePath = newPath;
                        bookModel.FilePath = newPath;
                    }

                    existing.Title = bookModel.Title;
                    existing.Description = bookModel.Description;
                    existing.Category = bookModel.Category;
                    existing.Price = (decimal)bookModel.Price;
                    existing.Status = bookModel.Status switch
                    {
                        "Approved" => true,
                        "Rejected" => false,
                        "Pending" => null,
                        _ => null
                    };

                    Task.Run(async () => await _bookRepository.UpdateAsync(existing)).Wait();
                }
            }
            catch (Exception ex)
            {
                var baseEx = ex.InnerException ?? ex;
                var innerMsg = baseEx.InnerException?.Message ?? baseEx.Message;
                throw new Exception($"Không thể cập nhật sách. Chi tiết lỗi CSDL: {innerMsg}", baseEx);
            }
        }

        public void ApproveBook(int bookId)
        {
            string? guid = GetGuidFromInt(bookId);
            if (string.IsNullOrEmpty(guid))
            {
                throw new BusinessRuleException("Book does not exist.");
            }

            string adminId = UserSession.CurrentUser?.AccountId
                ?? throw new BusinessRuleException("An authenticated admin is required.");

            Task.Run(() => _approvalService.ApproveBookAsync(guid, adminId)).GetAwaiter().GetResult();
        }

        public void RejectBook(int bookId, string comment)
        {
            string? guid = GetGuidFromInt(bookId);
            if (string.IsNullOrEmpty(guid))
            {
                throw new BusinessRuleException("Book does not exist.");
            }

            string adminId = UserSession.CurrentUser?.AccountId
                ?? throw new BusinessRuleException("An authenticated admin is required.");

            Task.Run(() => _approvalService.RejectBookAsync(guid, adminId, comment)).GetAwaiter().GetResult();
        }

        public void DeleteBook(int bookId)
        {
            try
            {
                string? guid = GetGuidFromInt(bookId);
                if (string.IsNullOrEmpty(guid)) return;

                using (var dbContext = new ProjectPrnContext())
                {
                    var existing = dbContext.Books.FirstOrDefault(b => b.BookId == guid);
                    if (existing != null)
                    {
                        dbContext.Books.Remove(existing);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var baseEx = ex.InnerException ?? ex;
                var innerMsg = baseEx.InnerException?.Message ?? baseEx.Message;
                throw new Exception($"Không thể xóa sách. Chi tiết lỗi CSDL: {innerMsg}", baseEx);
            }
        }
    }
}
