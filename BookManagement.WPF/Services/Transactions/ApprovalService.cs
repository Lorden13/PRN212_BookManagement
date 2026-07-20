using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookManagement.WPF.Services.Transactions;

public sealed class ApprovalService : IApprovalService
{
    private readonly ProjectPrnContext _context;

    public ApprovalService(ProjectPrnContext context) => _context = context;

    public async Task SubmitBookAsync(string bookId, string authorId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);

        var book = await _context.Books.SingleOrDefaultAsync(
            b => b.BookId == bookId && b.AuthorId == authorId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy sách hoặc tác giả không sở hữu sách này.");

        if (book.Status == true)
            throw new InvalidOperationException("Sách đã được duyệt không thể gửi duyệt lại.");
        if (book.Status is null)    
            return;

        book.Status = null;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<BookApproval> ApproveAsync(string bookId, string adminId, string? feedback = null, CancellationToken cancellationToken = default) =>
        ReviewAsync(bookId, adminId, true, feedback?.Trim() ?? string.Empty, cancellationToken);

    public Task<BookApproval> RejectAsync(string bookId, string adminId, string feedback, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(feedback))
            throw new ArgumentException("Phải nhập lý do từ chối.", nameof(feedback));

        return ReviewAsync(bookId, adminId, false, feedback.Trim(), cancellationToken);
    }

    public async Task<IReadOnlyList<BookApproval>> GetHistoryAsync(string bookId, CancellationToken cancellationToken = default) =>
        await _context.BookApprovals.AsNoTracking()
            .Include(a => a.Admin)
            .Include(a => a.Book)
            .Where(a => a.BookId == bookId)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync(cancellationToken);

    private async Task<BookApproval> ReviewAsync(string bookId, string adminId, bool approved, string feedback, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);
        ArgumentException.ThrowIfNullOrWhiteSpace(adminId);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        if (!await _context.Admins.AnyAsync(a => a.AdminId == adminId, cancellationToken))
            throw new InvalidOperationException("Tài khoản không có quyền Admin.");

        var book = await _context.Books.SingleOrDefaultAsync(b => b.BookId == bookId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy sách.");
        if (book.Status is not null)
            throw new InvalidOperationException("Chỉ sách đang chờ duyệt mới có thể được xử lý.");

        book.Status = approved;
        var approval = new BookApproval
        {
            ApprovalId = Guid.NewGuid().ToString(),
            BookId = bookId,
            AdminId = adminId,
            IsApproved = approved,
            Feedback = feedback,
            ActionDate = DateTime.UtcNow
        };
        _context.BookApprovals.Add(approval);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return approval;
    }
}
