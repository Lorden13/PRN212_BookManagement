namespace BookManagement.Models.Entities
{
    public class PurchaseModel
    {
        public string Id { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string ReaderId { get; set; } = string.Empty;
        public string ReaderName { get; set; } = string.Empty;
        public double Price { get; set; }
        public string PurchaseDate { get; set; } = string.Empty;
        public string Status { get; set; } = "Completed"; // Completed, Processing, Cancelled
    }
}
