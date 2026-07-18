using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;

namespace BookManagement.WPF.Services.Transactions;

public sealed class PurchaseTransactionService : IPurchaseTransactionService
{
    private readonly ProjectPrnContext _context;

    public PurchaseTransactionService(ProjectPrnContext context) => _context = context;

    public async Task<Purchase> PurchaseAsync(string readerId, string bookId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        if (!await _context.Readers.AnyAsync(r => r.ReaderId == readerId, cancellationToken))
            throw new InvalidOperationException("Không tìm thấy Reader.");

        var book = await _context.Books.SingleOrDefaultAsync(
            b => b.BookId == bookId && b.Status == true, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Chỉ có thể mua sách đã được duyệt.");
        if (book.Price < 0)
            throw new InvalidOperationException("Giá sách không hợp lệ.");

        var existing = await _context.Purchases.SingleOrDefaultAsync(
            p => p.ReaderId == readerId && p.BookId == bookId && p.IsBought, cancellationToken);
        if (existing is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            return existing;
        }

        var purchase = new Purchase
        {
            PurchaseId = Guid.NewGuid().ToString(),
            ReaderId = readerId,
            BookId = bookId,
            DownloadToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(),
            IsBought = true,
           // Payment = book.Price,
            PurchasedAt = DateTime.UtcNow
        };
        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return purchase;
    }

    public Task<bool> CanDownloadAsync(string readerId, string bookId, string downloadToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(readerId) || string.IsNullOrWhiteSpace(bookId) || string.IsNullOrWhiteSpace(downloadToken))
            return Task.FromResult(false);

        return _context.Purchases.AsNoTracking().AnyAsync(p =>
                p.ReaderId == readerId && p.BookId == bookId &&
                p.DownloadToken == downloadToken && p.IsBought,
            cancellationToken);
    }

    public async Task<string> GetAuthorizedFilePathAsync(
        string readerId,
        string bookId,
        string downloadToken,
        CancellationToken cancellationToken = default)
    {
        var filePath = await _context.Purchases.AsNoTracking()
            .Where(p => p.ReaderId == readerId && p.BookId == bookId &&
                        p.DownloadToken == downloadToken && p.IsBought)
            .Select(p => p.Book.FilePath)
            .SingleOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new UnauthorizedAccessException("Bạn chưa mua sách hoặc token tải xuống không hợp lệ.");

        return filePath;
    }

    public async Task<IReadOnlyList<Purchase>> GetHistoryAsync(string readerId, CancellationToken cancellationToken = default) =>
        await _context.Purchases.AsNoTracking().Include(p => p.Book)
            .Where(p => p.ReaderId == readerId && p.IsBought)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync(cancellationToken);
}
