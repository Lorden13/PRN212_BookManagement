using System;
using System.Collections.Generic;

namespace BookManagement.WPF.Entities;

public partial class Reader
{
    public string ReaderId { get; set; } = null!;

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual Account ReaderNavigation { get; set; } = null!;
}
