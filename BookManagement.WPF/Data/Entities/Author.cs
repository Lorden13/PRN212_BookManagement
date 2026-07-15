using System;
using System.Collections.Generic;

namespace BookManagement.Data.Entities;

public partial class Author
{
    public string AuthorId { get; set; } = null!;

    public virtual Account AuthorNavigation { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
