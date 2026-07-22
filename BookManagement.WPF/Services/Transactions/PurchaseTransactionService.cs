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

        var book = await _context.Books
            .Include(b => b.StockInfo)
            .SingleOrDefaultAsync(
            b => b.BookId == bookId && b.Status == true, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Chỉ có thể mua sách đã được duyệt.");
        if (book.Price < 0)
            throw new InvalidOperationException("Giá sách không hợp lệ.");

        var existing = await _context.Purchases.SingleOrDefaultAsync(
            p => p.ReaderId == readerId && p.BookId == bookId && p.IsBought, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Bạn đã mua sách này rồi.");
        if (book.StockInfo is null)
            throw new InvalidOperationException("Chưa có dữ liệu tồn kho cho sách này.");
        if (book.StockInfo.Quantity <= 0)
            throw new InvalidOperationException("Sách đã hết hàng.");

        var purchase = await _context.Purchases.FirstOrDefaultAsync(
            p => p.ReaderId == readerId && p.BookId == bookId && !p.IsBought,
            cancellationToken);
        if (purchase is null)
        {
            purchase = CreateCartPurchase(readerId, bookId);
            _context.Purchases.Add(purchase);
        }

        purchase.IsBought = true;
        purchase.PurchasedAt = DateTime.UtcNow;
        book.StockInfo!.Quantity -= 1;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return purchase;
    }

    public async Task<bool> AddToCartAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        if (!await _context.Readers.AnyAsync(r => r.ReaderId == readerId, cancellationToken))
            throw new InvalidOperationException("Không tìm thấy độc giả.");
        var book = await _context.Books
            .Include(b => b.StockInfo)
            .SingleOrDefaultAsync(
            b => b.BookId == bookId && b.Status == true, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Chỉ có thể thêm sách đã được duyệt vào giỏ hàng.");
        if (book.StockInfo is null)
            throw new InvalidOperationException("Chưa có dữ liệu tồn kho cho sách này.");
        if (book.StockInfo.Quantity <= 0)
            throw new InvalidOperationException("Sách đã hết hàng.");
        if (await _context.Purchases.AnyAsync(
                p => p.ReaderId == readerId && p.BookId == bookId,
                cancellationToken))
        {
            await transaction.CommitAsync(cancellationToken);
            return false;
        }

        _context.Purchases.Add(CreateCartPurchase(readerId, bookId));
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveFromCartAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

        var item = await _context.Purchases.FirstOrDefaultAsync(
            p => p.ReaderId == readerId && p.BookId == bookId && !p.IsBought,
            cancellationToken);
        if (item is null) return false;

        _context.Purchases.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Purchase>> GetCartAsync(
        string readerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);

        return await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Book)
            .ThenInclude(b => b.Author)
            .ThenInclude(a => a.AuthorNavigation)
            .Where(p => p.ReaderId == readerId && !p.IsBought && p.Book.Status == true)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> CheckoutAsync(
        string readerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var cartItems = await _context.Purchases
            .Include(p => p.Book)
            .ThenInclude(b => b.StockInfo)
            .Where(p => p.ReaderId == readerId && !p.IsBought && p.Book.Status == true)
            .ToListAsync(cancellationToken);
        if (cartItems.Count < 2)
            throw new InvalidOperationException("Giỏ hàng cần ít nhất 2 sách để thanh toán.");
        var missingStockItem = cartItems.FirstOrDefault(item => item.Book.StockInfo is null);
        if (missingStockItem is not null)
            throw new InvalidOperationException($"Chưa có dữ liệu tồn kho cho sách '{missingStockItem.Book.Title}'.");
        var outOfStockItem = cartItems.FirstOrDefault(item => item.Book.StockInfo!.Quantity <= 0);
        if (outOfStockItem is not null)
            throw new InvalidOperationException($"Sách '{outOfStockItem.Book.Title}' đã hết hàng.");

        var purchasedAt = DateTime.UtcNow;
        foreach (var item in cartItems)
        {
            item.IsBought = true;
            item.PurchasedAt = purchasedAt;
            item.Book.StockInfo!.Quantity -= 1;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return cartItems;
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

    //public async Task<string> GetAuthorizedFilePathAsync(
    //    string readerId,
    //    string bookId,
    //    string downloadToken,
    //    CancellationToken cancellationToken = default)
    //{
    //    //var filePath = await _context.Purchases.AsNoTracking()
    //    //    .Where(p => p.ReaderId == readerId && p.BookId == bookId &&
    //    //                p.DownloadToken == downloadToken && p.IsBought)
    //    //    .Select(p => p.Book.FilePath)
    //    //    .SingleOrDefaultAsync(cancellationToken);

    //    if (string.IsNullOrWhiteSpace(filePath))
    //        throw new UnauthorizedAccessException("Bạn chưa mua sách hoặc token tải xuống không hợp lệ.");

    //    return filePath;
    //}

    public async Task<IReadOnlyList<Purchase>> GetHistoryAsync(string readerId, CancellationToken cancellationToken = default) =>
        await _context.Purchases.AsNoTracking().Include(p => p.Book)
            .Where(p => p.ReaderId == readerId && p.IsBought)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync(cancellationToken);

    private static Purchase CreateCartPurchase(string readerId, string bookId) => new()
    {
        PurchaseId = Guid.NewGuid().ToString(),
        ReaderId = readerId,
        BookId = bookId,
        DownloadToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(),
        IsBought = false,
        PurchasedAt = DateTime.UtcNow
    };
}
