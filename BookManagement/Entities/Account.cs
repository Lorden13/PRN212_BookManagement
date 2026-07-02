using System;
using System.Collections.Generic;

namespace BookManagement.Entities;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Author? Author { get; set; }

    public virtual Reader? Reader { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
