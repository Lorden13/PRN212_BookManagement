using System;
using System.Collections.Generic;

namespace BookManagement.Entities;

public partial class Book
{
    public string BookId { get; set; } = null!;

    public string AuthorId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string FilePath { get; set; } = null!;

    public bool? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<BookApproval> BookApprovals { get; set; } = new List<BookApproval>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
