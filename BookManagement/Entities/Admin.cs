using System;
using System.Collections.Generic;

namespace BookManagement.Entities;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public virtual Account AdminNavigation { get; set; } = null!;

    public virtual ICollection<BookApproval> BookApprovals { get; set; } = new List<BookApproval>();
}
