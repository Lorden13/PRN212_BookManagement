using System;
using System.Collections.Generic;

namespace BookManagement.Data.Entities;

public partial class Favorite
{
    public string ReaderId { get; set; } = null!;

    public string BookId { get; set; } = null!;

    public DateTime? AddedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
