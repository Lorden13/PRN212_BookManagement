using System;
using System.Collections.Generic;

namespace BookManagement.WPF.Entities;

public partial class BookApproval
{
    public string ApprovalId { get; set; } = null!;

    public string BookId { get; set; } = null!;

    public string AdminId { get; set; } = null!;

    public bool IsApproved { get; set; }

    public string Feedback { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
