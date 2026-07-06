using System;
using System.Collections.Generic;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.Helpers
{
    public static class MockDataService
    {
        public static List<User> Users { get; } = new()
        {
            new User { Id = 1, FullName = "Admin User", Email = "admin@book.com", Password = "123456", Role = UserRole.Admin, Status = UserStatus.Active, Address = "123 Main St", PhoneNumber = "0987654321" },
            new User { Id = 2, FullName = "John Writer", Email = "author@book.com", Password = "123456", Role = UserRole.Author, Status = UserStatus.Active, Address = "456 Writer Ave", PhoneNumber = "0912345678" },
            new User { Id = 3, FullName = "Jane Reader", Email = "reader@book.com", Password = "123456", Role = UserRole.Reader, Status = UserStatus.Active, Address = "789 Reader Rd", PhoneNumber = "0900112233" },
            new User { Id = 4, FullName = "Emily Novelist", Email = "emily@book.com", Password = "123456", Role = UserRole.Author, Status = UserStatus.Active, Address = "101 Story Ln", PhoneNumber = "0944556677" },
            new User { Id = 5, FullName = "Michael Brown", Email = "michael@book.com", Password = "123456", Role = UserRole.Reader, Status = UserStatus.Active, Address = "202 Maple St", PhoneNumber = "0955667788" },
            new User { Id = 6, FullName = "Sarah Johnson", Email = "sarah@book.com", Password = "123456", Role = UserRole.Reader, Status = UserStatus.Inactive, Address = "303 Pine Rd", PhoneNumber = "0966778899" },
            new User { Id = 7, FullName = "David Lee", Email = "david@book.com", Password = "123456", Role = UserRole.Author, Status = UserStatus.Active, Address = "404 Cedar St", PhoneNumber = "0977889900" },
        };

        public static List<Book> Books { get; } = new()
        {
            new Book { Id = 1, Title = "The Art of Clean Code", AuthorName = "John Writer", Category = "Technology", Description = "A comprehensive guide to writing clean, maintainable code that stands the test of time.", Price = 29.99m, CoverColor = "#2563EB", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 1, 15), AuthorId = 2 },
            new Book { Id = 2, Title = "Digital Horizons", AuthorName = "John Writer", Category = "Technology", Description = "Exploring the future of digital transformation and its impact on modern society.", Price = 24.99m, CoverColor = "#7C3AED", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 2, 20), AuthorId = 2 },
            new Book { Id = 3, Title = "Midnight Garden", AuthorName = "Emily Novelist", Category = "Fiction", Description = "A magical tale of mystery and wonder set in a secret garden that only blooms at midnight.", Price = 19.99m, CoverColor = "#059669", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 3, 10), AuthorId = 4 },
            new Book { Id = 4, Title = "The Silent Ocean", AuthorName = "Emily Novelist", Category = "Fiction", Description = "An epic adventure across uncharted waters, where silence holds the greatest secrets.", Price = 22.99m, CoverColor = "#0891B2", Status = BookStatus.Pending, CreatedDate = new DateTime(2024, 4, 5), AuthorId = 4 },
            new Book { Id = 5, Title = "Startup Mindset", AuthorName = "John Writer", Category = "Business", Description = "Essential strategies for building a successful startup from the ground up.", Price = 34.99m, CoverColor = "#DC2626", Status = BookStatus.Pending, CreatedDate = new DateTime(2024, 5, 12), AuthorId = 2 },
            new Book { Id = 6, Title = "Whispers in the Wind", AuthorName = "David Lee", Category = "Poetry", Description = "A beautiful collection of poems about nature, love, and the passage of time.", Price = 14.99m, CoverColor = "#EA580C", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 6, 1), AuthorId = 7 },
            new Book { Id = 7, Title = "Data Science Essentials", AuthorName = "John Writer", Category = "Technology", Description = "A beginner-friendly introduction to data science concepts and tools.", Price = 39.99m, CoverColor = "#4F46E5", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 6, 18), AuthorId = 2 },
            new Book { Id = 8, Title = "The Last Kingdom", AuthorName = "Emily Novelist", Category = "History", Description = "A historical fiction novel set during the Viking age in medieval England.", Price = 27.99m, CoverColor = "#B91C1C", Status = BookStatus.Rejected, CreatedDate = new DateTime(2024, 7, 3), AuthorId = 4 },
            new Book { Id = 9, Title = "Cooking with Love", AuthorName = "David Lee", Category = "Lifestyle", Description = "Over 100 delicious recipes inspired by family traditions from around the world.", Price = 32.99m, CoverColor = "#C2410C", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 7, 22), AuthorId = 7 },
            new Book { Id = 10, Title = "Mindful Living", AuthorName = "David Lee", Category = "Self-Help", Description = "Practical tips for incorporating mindfulness into your daily routine.", Price = 18.99m, CoverColor = "#0D9488", Status = BookStatus.Pending, CreatedDate = new DateTime(2024, 8, 10), AuthorId = 7 },
            new Book { Id = 11, Title = "Beyond the Stars", AuthorName = "Emily Novelist", Category = "Science Fiction", Description = "A thrilling sci-fi adventure about humanity's first contact with alien civilizations.", Price = 25.99m, CoverColor = "#6D28D9", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 8, 25), AuthorId = 4 },
            new Book { Id = 12, Title = "Financial Freedom", AuthorName = "John Writer", Category = "Business", Description = "A step-by-step guide to achieving financial independence through smart investing.", Price = 28.99m, CoverColor = "#15803D", Status = BookStatus.Approved, CreatedDate = new DateTime(2024, 9, 5), AuthorId = 2 },
        };

        public static List<FavoriteBook> FavoriteBooks { get; } = new()
        {
            new FavoriteBook { Id = 1, UserId = 3, BookId = 1, AddedDate = new DateTime(2024, 3, 1) },
            new FavoriteBook { Id = 2, UserId = 3, BookId = 3, AddedDate = new DateTime(2024, 4, 15) },
            new FavoriteBook { Id = 3, UserId = 3, BookId = 6, AddedDate = new DateTime(2024, 5, 20) },
            new FavoriteBook { Id = 4, UserId = 3, BookId = 11, AddedDate = new DateTime(2024, 9, 1) },
            new FavoriteBook { Id = 5, UserId = 5, BookId = 2, AddedDate = new DateTime(2024, 6, 10) },
            new FavoriteBook { Id = 6, UserId = 5, BookId = 7, AddedDate = new DateTime(2024, 7, 5) },
        };

        public static List<PurchaseRecord> PurchaseRecords { get; } = new()
        {
            new PurchaseRecord { Id = 1, BookTitle = "The Art of Clean Code", PurchaseDate = new DateTime(2024, 3, 5), Price = 29.99m, Status = "Completed" },
            new PurchaseRecord { Id = 2, BookTitle = "Midnight Garden", PurchaseDate = new DateTime(2024, 4, 20), Price = 19.99m, Status = "Completed" },
            new PurchaseRecord { Id = 3, BookTitle = "Whispers in the Wind", PurchaseDate = new DateTime(2024, 6, 10), Price = 14.99m, Status = "Completed" },
            new PurchaseRecord { Id = 4, BookTitle = "Beyond the Stars", PurchaseDate = new DateTime(2024, 9, 15), Price = 25.99m, Status = "Processing" },
            new PurchaseRecord { Id = 5, BookTitle = "Data Science Essentials", PurchaseDate = new DateTime(2024, 8, 1), Price = 39.99m, Status = "Completed" },
        };

        public static List<ApprovalRecord> ApprovalRecords { get; } = new()
        {
            new ApprovalRecord { Id = 1, BookTitle = "The Art of Clean Code", AuthorName = "John Writer", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 1, 18) },
            new ApprovalRecord { Id = 2, BookTitle = "Digital Horizons", AuthorName = "John Writer", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 2, 25) },
            new ApprovalRecord { Id = 3, BookTitle = "Midnight Garden", AuthorName = "Emily Novelist", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 3, 15) },
            new ApprovalRecord { Id = 4, BookTitle = "The Last Kingdom", AuthorName = "Emily Novelist", Decision = "Rejected", ReviewedBy = "Admin User", Date = new DateTime(2024, 7, 10), RejectionReason = "Content needs revision. Historical inaccuracies found in chapters 3-5." },
            new ApprovalRecord { Id = 5, BookTitle = "Whispers in the Wind", AuthorName = "David Lee", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 6, 5) },
            new ApprovalRecord { Id = 6, BookTitle = "Data Science Essentials", AuthorName = "John Writer", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 6, 22) },
            new ApprovalRecord { Id = 7, BookTitle = "Beyond the Stars", AuthorName = "Emily Novelist", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 8, 28) },
            new ApprovalRecord { Id = 8, BookTitle = "Cooking with Love", AuthorName = "David Lee", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 7, 25) },
            new ApprovalRecord { Id = 9, BookTitle = "Financial Freedom", AuthorName = "John Writer", Decision = "Approved", ReviewedBy = "Admin User", Date = new DateTime(2024, 9, 8) },
        };

        public static User? Authenticate(string email, string password)
        {
            return Users.Find(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }
    }
}
