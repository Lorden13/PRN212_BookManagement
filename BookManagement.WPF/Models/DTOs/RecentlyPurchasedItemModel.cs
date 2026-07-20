namespace BookManagement.Models.DTOs;

public class RecentlyPurchasedItemModel
{
    public string BookId { get; set; }

    public string BookTitle { get; set; }

    public string Author { get; set; }

    public string PurchaseDate { get; set; }

    public double Price { get; set; }

    public string Status { get; set; }

    public string CoverImagePath { get; set; }
}