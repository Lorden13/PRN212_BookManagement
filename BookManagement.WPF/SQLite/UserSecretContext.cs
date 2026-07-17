using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.SQLite;

public partial class UserSecretContext : DbContext
{
    public UserSecretContext()
    {
        Database.EnsureCreated();
    }

    public UserSecretContext(DbContextOptions<UserSecretContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SavedToken> SavedTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=UserSecret.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavedToken>(entity =>
        {
            entity.ToTable("SavedToken");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
