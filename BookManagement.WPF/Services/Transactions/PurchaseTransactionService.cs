using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.WPF.Services.Transactions;

public sealed class PurchaseTransactionService : IPurchaseTransactionService
{
    private readonly ProjectPrnContext _context;

    public PurchaseTransactionService(ProjectPrnContext context) => _context = context;

    public async Task<Purchase> PurchaseAsync(string readerId, string bookId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        if (!await _context.Readers.AnyAsync(r => r.ReaderId == readerId, cancellationToken))
            throw new InvalidOperationException("Không tìm thấy Reader.");
        if (!await _context.Books.AnyAsync(b => b.BookId == bookId && b.Status == true, cancellationToken))
            throw new InvalidOperationException("Chỉ có thể mua sách đã được duyệt.");

        var existing = await _context.Purchases.SingleOrDefaultAsync(
            p => p.ReaderId == readerId && p.BookId == bookId && p.IsBought, cancellationToken);
        if (existing is not null)
            return existing;

        var purchase = new Purchase
        {
            PurchaseId = Guid.NewGuid().ToString(),
            ReaderId = readerId,
            BookId = bookId,
            DownloadToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(),
            IsBought = true,
            PurchasedAt = DateTime.Now
        };
        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return purchase;
    }

    public Task<bool> CanDownloadAsync(string readerId, string bookId, string downloadToken, CancellationToken cancellationToken = default) =>
        _context.Purchases.AsNoTracking().AnyAsync(p =>
            p.ReaderId == readerId && p.BookId == bookId && p.DownloadToken == downloadToken && p.IsBought,
            cancellationToken);

    public async Task<IReadOnlyList<Purchase>> GetHistoryAsync(string readerId, CancellationToken cancellationToken = default) =>
        await _context.Purchases.AsNoTracking().Include(p => p.Book)
            .Where(p => p.ReaderId == readerId && p.IsBought)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync(cancellationToken);
}
