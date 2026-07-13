using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.WPF.Services.Transactions;

public sealed class ApprovalService : IApprovalService
{
    private readonly ProjectPrnContext _context;

    public ApprovalService(ProjectPrnContext context) => _context = context;

    public async Task SubmitBookAsync(string bookId, string authorId, CancellationToken cancellationToken = default)
    {
        var book = await _context.Books.SingleOrDefaultAsync(
            b => b.BookId == bookId && b.AuthorId == authorId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy sách hoặc tác giả không sở hữu sách này.");

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
            .Where(a => a.BookId == bookId)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync(cancellationToken);

    private async Task<BookApproval> ReviewAsync(string bookId, string adminId, bool approved, string feedback, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var book = await _context.Books.SingleOrDefaultAsync(b => b.BookId == bookId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy sách.");
        if (book.Status is not null)
            throw new InvalidOperationException("Chỉ sách đang chờ duyệt mới có thể được xử lý.");
        if (!await _context.Admins.AnyAsync(a => a.AdminId == adminId, cancellationToken))
            throw new InvalidOperationException("Tài khoản không có quyền Admin.");

        book.Status = approved;
        var approval = new BookApproval
        {
            ApprovalId = Guid.NewGuid().ToString(),
            BookId = bookId,
            AdminId = adminId,
            IsApproved = approved,
            Feedback = feedback,
            ActionDate = DateTime.Now
        };
        _context.BookApprovals.Add(approval);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return approval;
    }
}
