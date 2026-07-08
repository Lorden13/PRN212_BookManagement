using System;
using System.Collections.Generic;

namespace BookManagement.Entities;

public partial class Purchase
{
    public string PurchaseId { get; set; } = null!;

    public string ReaderId { get; set; } = null!;

    public string BookId { get; set; } = null!;

    public string DownloadToken { get; set; } = null!;

    public bool IsBought { get; set; }

    public DateTime PurchasedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
