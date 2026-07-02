namespace BookManagement.Models.Entities
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty; // Approved, Rejected
        public string AdminComment { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
