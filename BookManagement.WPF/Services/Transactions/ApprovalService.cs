using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public sealed class ApprovalService : IApprovalService
{
    private readonly Func<ProjectPrnContext> _contextFactory;

    public ApprovalService()
        : this(() => new ProjectPrnContext())
    {
    }

    public ApprovalService(Func<ProjectPrnContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

      public async Task<Book> SubmitBookAsync(
        string bookId,
        string authorId,
        CancellationToken cancellationToken = default)
    {
        RequireId(bookId, nameof(bookId));
        RequireId(authorId, nameof(authorId));

        await using ProjectPrnContext context = _contextFactory();
        Book? book = await context.Books
            .SingleOrDefaultAsync(
                item => item.BookId == bookId && item.AuthorId == authorId,
                cancellationToken);

        if (book is null)
        {
            throw new BusinessRuleException("Book does not exist or does not belong to this author.");
        }

        bool authorIsActive = await context.Authors
            .Where(author => author.AuthorId == authorId)
            .Select(author => author.AuthorNavigation.IsActive)
            .SingleOrDefaultAsync(cancellationToken);

        if (!authorIsActive)
        {
            throw new BusinessRuleException("The author account is inactive.");
        }

        if (book.Status is true)
        {
            throw new BusinessRuleException("An approved book cannot be submitted again.");
        }

        if (book.Status is false)
        {
            book.Status = null;
            await context.SaveChangesAsync(cancellationToken);
        }

        return book;
    }

    public Task<BookApproval> ApproveBookAsync(
        string bookId,
        string adminId,
        string? feedback = null,
        CancellationToken cancellationToken = default) =>
        ReviewBookAsync(bookId, adminId, true, feedback, cancellationToken);

    public Task<BookApproval> RejectBookAsync(
        string bookId,
        string adminId,
        string feedback,
        CancellationToken cancellationToken = default) =>
        ReviewBookAsync(bookId, adminId, false, feedback, cancellationToken);

    public async Task<BookApproval> UpdateFeedbackAsync(
        string approvalId,
        string adminId,
        string feedback,
        CancellationToken cancellationToken = default)
    {
        RequireId(approvalId, nameof(approvalId));
        RequireId(adminId, nameof(adminId));

        await using ProjectPrnContext context = _contextFactory();
        BookApproval? approval = await context.BookApprovals
            .Include(item => item.Admin)
            .ThenInclude(admin => admin.AdminNavigation)
            .SingleOrDefaultAsync(
                item => item.ApprovalId == approvalId && item.AdminId == adminId,
                cancellationToken);

        if (approval is null)
        {
            throw new BusinessRuleException("Approval record does not exist or belongs to another admin.");
        }

        if (!approval.Admin.AdminNavigation.IsActive)
        {
            throw new BusinessRuleException("The admin account is inactive.");
        }

        string normalizedFeedback = NormalizeFeedback(feedback);
        if (!approval.IsApproved && normalizedFeedback.Length == 0)
        {
            throw new BusinessRuleException("Feedback is required for a rejected book.");
        }

        approval.Feedback = normalizedFeedback;
        await context.SaveChangesAsync(cancellationToken);
        return approval;
    }

    public async Task<IReadOnlyList<Book>> GetPendingBooksAsync(
        CancellationToken cancellationToken = default)
    {
        await using ProjectPrnContext context = _contextFactory();
        return await context.Books
            .AsNoTracking()
            .Where(book => book.Status == null)
            .Include(book => book.Author)
            .ThenInclude(author => author.AuthorNavigation)
            .OrderBy(book => book.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookApproval>> GetBookHistoryAsync(
        string bookId,
        CancellationToken cancellationToken = default)
    {
        RequireId(bookId, nameof(bookId));

        await using ProjectPrnContext context = _contextFactory();
        return await ApprovalHistoryQuery(context)
            .Where(approval => approval.BookId == bookId)
            .OrderByDescending(approval => approval.ActionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookApproval>> GetAuthorHistoryAsync(
        string authorId,
        CancellationToken cancellationToken = default)
    {
        RequireId(authorId, nameof(authorId));

        await using ProjectPrnContext context = _contextFactory();
        return await ApprovalHistoryQuery(context)
            .Where(approval => approval.Book.AuthorId == authorId)
            .OrderByDescending(approval => approval.ActionDate)
            .ToListAsync(cancellationToken);
    }

    private async Task<BookApproval> ReviewBookAsync(
        string bookId,
        string adminId,
        bool isApproved,
        string? feedback,
        CancellationToken cancellationToken)
    {
        RequireId(bookId, nameof(bookId));
        RequireId(adminId, nameof(adminId));

        string normalizedFeedback = NormalizeFeedback(feedback);
        if (!isApproved && normalizedFeedback.Length == 0)
        {
            throw new BusinessRuleException("Feedback is required when rejecting a book.");
        }

        await using ProjectPrnContext context = _contextFactory();
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        bool adminIsActive = await context.Admins
            .Where(admin => admin.AdminId == adminId)
            .Select(admin => admin.AdminNavigation.IsActive)
            .SingleOrDefaultAsync(cancellationToken);

        if (!adminIsActive)
        {
            throw new BusinessRuleException("Admin does not exist or the account is inactive.");
        }

        Book? book = await context.Books
            .SingleOrDefaultAsync(item => item.BookId == bookId, cancellationToken);

        if (book is null)
        {
            throw new BusinessRuleException("Book does not exist.");
        }

        if (book.Status is not null)
        {
            throw new BusinessRuleException("Only a pending book can be approved or rejected.");
        }

        BookApproval approval = new()
        {
            ApprovalId = Guid.NewGuid().ToString(),
            BookId = bookId,
            AdminId = adminId,
            IsApproved = isApproved,
            Feedback = normalizedFeedback,
            ActionDate = DateTime.UtcNow
        };

        book.Status = isApproved;
        await context.BookApprovals.AddAsync(approval, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return approval;
    }

    private static IQueryable<BookApproval> ApprovalHistoryQuery(ProjectPrnContext context) =>
        context.BookApprovals
            .AsNoTracking()
            .Include(approval => approval.Book)
            .ThenInclude(book => book.Author)
            .ThenInclude(author => author.AuthorNavigation)
            .Include(approval => approval.Admin)
            .ThenInclude(admin => admin.AdminNavigation);

    private static string NormalizeFeedback(string? feedback) => feedback?.Trim() ?? string.Empty;

    private static void RequireId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty ID is required.", parameterName);
        }
    }
}
