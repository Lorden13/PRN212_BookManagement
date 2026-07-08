using System;

namespace BookManagement.WPF.Models
{
    public class PurchaseRecord
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string ReaderName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Completed";
    }
}
