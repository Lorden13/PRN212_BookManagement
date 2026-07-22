using System;

namespace BookManagement.WPF.Models
{
    public enum BookStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CoverColor { get; set; } = "#2563EB";
        public BookStatus Status { get; set; } = BookStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int AuthorId { get; set; }
    //    public bool IsDeleted { get; set; } = false;
    }
}
