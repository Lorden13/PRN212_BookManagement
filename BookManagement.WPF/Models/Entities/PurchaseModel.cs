namespace BookManagement.Models.Entities
{
    public class PurchaseModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int ReaderId { get; set; }
        public string ReaderName { get; set; } = string.Empty;
        public double Price { get; set; }
        public string PurchaseDate { get; set; } = string.Empty;
        public string Status { get; set; } = "Completed"; // Completed, Processing, Cancelled
    }
}
