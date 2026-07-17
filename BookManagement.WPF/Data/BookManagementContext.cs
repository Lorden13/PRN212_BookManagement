using System;
using System.Collections.Generic;
using BookManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data;

public partial class BookManagementContext : DbContext
{
    public BookManagementContext(DbContextOptions<BookManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookApproval> BookApprovals { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA586532F3E2B");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__A9D105347103266B").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AccountID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__RoleID__286302EC");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.AdminId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AdminID");

            entity.HasOne(d => d.AdminNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Account");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");

            entity.Property(e => e.AuthorId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AuthorID");

            entity.HasOne(d => d.AuthorNavigation).WithOne(p => p.Author)
                .HasForeignKey<Author>(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Author_Account");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C227718669CE");

            entity.Property(e => e.BookId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("BookID");
            entity.Property(e => e.AuthorId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AuthorID");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Author");
        });

        modelBuilder.Entity<BookApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId);

            entity.Property(e => e.ApprovalId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("ApprovalID");
            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.AdminId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AdminID");
            entity.Property(e => e.BookId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("BookID");

            entity.HasOne(d => d.Admin).WithMany(p => p.BookApprovals)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookApprovals_Admin");

            entity.HasOne(d => d.Book).WithMany(p => p.BookApprovals)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookApprovals_Books");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.ReaderId, e.BookId }).HasName("PK__Favorite__EDB9A9A37E1A6D4D");

            entity.Property(e => e.ReaderId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("ReaderID");
            entity.Property(e => e.BookId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("BookID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__BookI__36B12243");

            entity.HasOne(d => d.Reader).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Reader");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BDE24464203");

            entity.HasIndex(e => e.DownloadToken, "UQ__Purchase__142F302BF4B81563").IsUnique();

            entity.HasIndex(e => new { e.ReaderId, e.BookId }, "UX_Purchases_Reader_Book_Bought")
                .IsUnique()
                .HasFilter("[IsBought] = 1");

            entity.Property(e => e.PurchaseId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("PurchaseID");
            entity.Property(e => e.BookId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("BookID");
            entity.Property(e => e.DownloadToken)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsBought).HasDefaultValue(true);
            entity.Property(e => e.Payment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PurchasedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReaderId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("ReaderID");

            entity.HasOne(d => d.Book).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__BookI__3E52440B");

            entity.HasOne(d => d.Reader).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Reader");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.ToTable("Reader");

            entity.Property(e => e.ReaderId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("ReaderID");

            entity.HasOne(d => d.ReaderNavigation).WithOne(p => p.Reader)
                .HasForeignKey<Reader>(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reader_Account");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A15522637");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Token__658FEE8A4BE30682");

            entity.ToTable("Token");

            entity.HasIndex(e => e.TokenValue, "UQ__Token__FE1B80ECF9BE8E36").IsUnique();

            entity.Property(e => e.TokenId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("TokenID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("AccountID");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.TokenValue)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Token__AccountID__2C3393D0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
