using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Mock
{
    public class MockBookService : IBookService
    {
        private readonly List<BookModel> _books = new List<BookModel>();

        public MockBookService()
        {
            // Seed 20+ realistic books
            _books.Add(new BookModel { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Category = "Programming", Price = 9.99, Status = "Approved", Rating = 4.8, Description = "A Handbook of Agile Software Craftsmanship.", CoverImagePath = "/Assets/Covers/clean_code.jpg", SubmittedDate = "2026-05-20" });
            _books.Add(new BookModel { Id = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Category = "Programming", Price = 12.99, Status = "Approved", Rating = 4.7, Description = "Your journey to mastery.", CoverImagePath = "/Assets/Covers/pragmatic.jpg", SubmittedDate = "2026-05-18" });
            _books.Add(new BookModel { Id = 3, Title = "Design Patterns", Author = "Erich Gamma", Category = "Programming", Price = 13.99, Status = "Approved", Rating = 4.6, Description = "Elements of Reusable Object-Oriented Software.", CoverImagePath = "/Assets/Covers/design_patterns.jpg", SubmittedDate = "2026-05-16" });
            _books.Add(new BookModel { Id = 4, Title = "Refactoring", Author = "Martin Fowler", Category = "Programming", Price = 11.49, Status = "Approved", Rating = 4.7, Description = "Improving the Design of Existing Code.", CoverImagePath = "/Assets/Covers/refactoring.jpg", SubmittedDate = "2026-05-14" });
            _books.Add(new BookModel { Id = 5, Title = "The Hobbit", Author = "J. R. R. Tolkien", Category = "Novel", Price = 7.99, Status = "Approved", Rating = 4.9, Description = "A great adventure novel.", CoverImagePath = "/Assets/Covers/hobbit.jpg", SubmittedDate = "2026-05-15" });
            _books.Add(new BookModel { Id = 6, Title = "Harry Potter and the Sorcerer's Stone", Author = "J. K. Rowling", Category = "Novel", Price = 8.99, Status = "Approved", Rating = 4.8, Description = "The start of the magical wizarding world.", CoverImagePath = "/Assets/Covers/hp1.jpg", SubmittedDate = "2026-05-19" });
            _books.Add(new BookModel { Id = 7, Title = "Harry Potter and the Chamber of Secrets", Author = "J. K. Rowling", Category = "Novel", Price = 9.99, Status = "Approved", Rating = 4.7, Description = "Book 2 of the series.", CoverImagePath = "/Assets/Covers/hp2.jpg", SubmittedDate = "2026-05-21" });
            _books.Add(new BookModel { Id = 8, Title = "Zero to One", Author = "Peter Thiel", Category = "Business", Price = 10.99, Status = "Approved", Rating = 4.5, Description = "Notes on Startups, or How to Build the Future.", CoverImagePath = "/Assets/Covers/zero_to_one.jpg", SubmittedDate = "2026-05-10" });
            _books.Add(new BookModel { Id = 9, Title = "The Lean Startup", Author = "Eric Ries", Category = "Business", Price = 11.99, Status = "Approved", Rating = 4.6, Description = "How Today's Entrepreneurs Use Continuous Innovation.", CoverImagePath = "/Assets/Covers/lean_startup.jpg", SubmittedDate = "2026-05-12" });
            _books.Add(new BookModel { Id = 10, Title = "A Brief History of Time", Author = "Stephen Hawking", Category = "Science", Price = 9.49, Status = "Approved", Rating = 4.8, Description = "From the Big Bang to Black Holes.", CoverImagePath = "/Assets/Covers/brief_history.jpg", SubmittedDate = "2026-05-08" });
            _books.Add(new BookModel { Id = 11, Title = "Cosmos", Author = "Carl Sagan", Category = "Science", Price = 10.50, Status = "Approved", Rating = 4.9, Description = "Exploration of the universe and humanity.", CoverImagePath = "/Assets/Covers/cosmos.jpg", SubmittedDate = "2026-05-05" });
            _books.Add(new BookModel { Id = 12, Title = "Sapiens: A Brief History of Humankind", Author = "Yuval Noah Harari", Category = "History", Price = 12.00, Status = "Approved", Rating = 4.7, Description = "A bold history of humanity.", CoverImagePath = "/Assets/Covers/sapiens.jpg", SubmittedDate = "2026-05-01" });
            _books.Add(new BookModel { Id = 13, Title = "Guns, Germs, and Steel", Author = "Jared Diamond", Category = "History", Price = 10.99, Status = "Approved", Rating = 4.5, Description = "The fates of human societies.", CoverImagePath = "/Assets/Covers/guns_germs.jpg", SubmittedDate = "2026-05-03" });
            _books.Add(new BookModel { Id = 14, Title = "The Elements of Style", Author = "William Strunk Jr.", Category = "Education", Price = 5.99, Status = "Approved", Rating = 4.4, Description = "Classic guide to English writing style.", CoverImagePath = "/Assets/Covers/elements_style.jpg", SubmittedDate = "2026-05-02" });
            _books.Add(new BookModel { Id = 15, Title = "Democracy in America", Author = "Alexis de Tocqueville", Category = "History", Price = 8.50, Status = "Approved", Rating = 4.6, Description = "Observations on American society and politics.", CoverImagePath = "/Assets/Covers/democracy.jpg", SubmittedDate = "2026-05-04" });

            // Pending Books
            _books.Add(new BookModel { Id = 16, Title = "C# Basics", Author = "Alice Johnson", Category = "Programming", Price = 6.99, Status = "Pending", Rating = 0.0, Description = "An introduction to C# programming.", CoverImagePath = "/Assets/Covers/csharp_basics.jpg", SubmittedDate = "2026-06-20 14:30" });
            _books.Add(new BookModel { Id = 17, Title = "ASP.NET Core Guide", Author = "Alice Johnson", Category = "Programming", Price = 9.99, Status = "Approved", Rating = 4.6, Description = "Build modern web APIs with ASP.NET Core.", CoverImagePath = "/Assets/Covers/aspnet_core.jpg", SubmittedDate = "2026-05-18 10:15" });
            _books.Add(new BookModel { Id = 18, Title = "Learn SQL Server", Author = "Alice Johnson", Category = "Programming", Price = 8.99, Status = "Approved", Rating = 4.5, Description = "Master relational database management.", CoverImagePath = "/Assets/Covers/sql_server.jpg", SubmittedDate = "2026-05-15 16:20" });
            _books.Add(new BookModel { Id = 19, Title = "Python Data Science", Author = "Alice Johnson", Category = "Programming", Price = 10.99, Status = "Rejected", Rating = 0.0, Description = "Data analysis and machine learning with Python.", CoverImagePath = "/Assets/Covers/python_ds.jpg", SubmittedDate = "2026-05-10 09:40" });
            _books.Add(new BookModel { Id = 20, Title = "Web Design with HTML & CSS", Author = "Alice Johnson", Category = "Programming", Price = 7.99, Status = "Approved", Rating = 4.4, Description = "HTML5 & CSS3 layout design basics.", CoverImagePath = "/Assets/Covers/html_css.jpg", SubmittedDate = "2026-05-08 11:05" });
            _books.Add(new BookModel { Id = 21, Title = "Java Programming", Author = "Alice Johnson", Category = "Programming", Price = 9.99, Status = "Pending", Rating = 0.0, Description = "Complete guide to Java enterprise development.", CoverImagePath = "/Assets/Covers/java.jpg", SubmittedDate = "2026-05-05 13:20" });
            _books.Add(new BookModel { Id = 23, Title = "Enterprise Architecture Patterns", Author = "Martin Fowler", Category = "Technology", Price = 19.99, Status = "Pending", Rating = 0.0, Description = "A guide to building enterprise applications.", CoverImagePath = "/Assets/Covers/ent_arch.jpg", SubmittedDate = "2026-06-21 09:30" });
            _books.Add(new BookModel { Id = 24, Title = "Refactoring Patterns", Author = "Martin Fowler", Category = "Technology", Price = 17.50, Status = "Pending", Rating = 0.0, Description = "Catalogs of code structures and smells.", CoverImagePath = "/Assets/Covers/refactor_patterns.jpg", SubmittedDate = "2026-06-22 10:15" });
            _books.Add(new BookModel { Id = 25, Title = "Agile Principles", Author = "Robert C. Martin", Category = "Technology", Price = 12.99, Status = "Pending", Rating = 0.0, Description = "Agile software development principles and practices.", CoverImagePath = "/Assets/Covers/agile_principles.jpg", SubmittedDate = "2026-06-22 11:45" });
            _books.Add(new BookModel { Id = 26, Title = "WPF Deep Dive", Author = "Alice Johnson", Category = "Technology", Price = 11.99, Status = "Pending", Rating = 0.0, Description = "Advanced techniques in WPF UI development.", CoverImagePath = "/Assets/Covers/wpf_deep_dive.jpg", SubmittedDate = "2026-06-23 08:30" });
            _books.Add(new BookModel { Id = 27, Title = "The Universe and Beyond", Author = "Carl Sagan", Category = "Science", Price = 14.99, Status = "Pending", Rating = 0.0, Description = "An inspection of the outer universe.", CoverImagePath = "/Assets/Covers/universe_beyond.jpg", SubmittedDate = "2026-06-23 15:40" });
            _books.Add(new BookModel { Id = 28, Title = "Harry Potter and the Goblet of Fire", Author = "J. K. Rowling", Category = "Novel", Price = 15.99, Status = "Pending", Rating = 0.0, Description = "Year 4 at Hogwarts wizarding school.", CoverImagePath = "/Assets/Covers/hp4.jpg", SubmittedDate = "2026-06-24 10:00" });
            _books.Add(new BookModel { Id = 29, Title = "Clean Code Handbook", Author = "Robert C. Martin", Category = "Programming", Price = 10.99, Status = "Pending", Rating = 0.0, Description = "Key checklists for everyday coding craftsmanship.", CoverImagePath = "/Assets/Covers/clean_handbook.jpg", SubmittedDate = "2026-06-24 16:15" });
            _books.Add(new BookModel { Id = 30, Title = "Introduction to Cosmos", Author = "Carl Sagan", Category = "Science", Price = 13.50, Status = "Pending", Rating = 0.0, Description = "Elementary cosmos study guide.", CoverImagePath = "/Assets/Covers/intro_cosmos.jpg", SubmittedDate = "2026-06-25 11:20" });

            // Drafts (Author)
            _books.Add(new BookModel { Id = 22, Title = "Deep Learning with PyTorch", Author = "Alice Johnson", Category = "Technology", Price = 14.99, Status = "Draft", Rating = 0.0, Description = "Advanced neural network architectures in PyTorch.", CoverImagePath = "/Assets/Covers/pytorch.jpg", SubmittedDate = "2026-06-25" });
        }

        public IEnumerable<BookModel> GetApprovedBooks() => _books.Where(b => b.Status == "Approved");

        public IEnumerable<BookModel> GetPendingBooks() => _books.Where(b => b.Status == "Pending");

        public IEnumerable<BookModel> GetMyBooks(int authorId) => _books; // Returns all books for demo purposes

        public BookModel GetBookById(int id) => _books.FirstOrDefault(b => b.Id == id);

        public void CreateBook(BookModel book)
        {
            book.Id = _books.Max(b => b.Id) + 1;
            _books.Add(book);
        }

        public void UpdateBook(BookModel book)
        {
            var existing = GetBookById(book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.Category = book.Category;
                existing.Price = book.Price;
                existing.Status = book.Status;
                existing.Description = book.Description;
                existing.CoverImagePath = book.CoverImagePath;
            }
        }

        public void ApproveBook(int bookId)
        {
            var book = GetBookById(bookId);
            if (book != null) book.Status = "Approved";
        }

        public void RejectBook(int bookId, string comment)
        {
            var book = GetBookById(bookId);
            if (book != null) book.Status = "Rejected";
        }
    }
}
