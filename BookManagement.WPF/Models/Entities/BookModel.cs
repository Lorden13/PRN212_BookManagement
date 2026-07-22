namespace BookManagement.Models.Entities
{
    public class BookModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = "Draft"; // Approved, Pending, Rejected, Draft
        public string CoverImagePath { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SubmittedDate { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public Guid BookId { get; set; }
    }
}
