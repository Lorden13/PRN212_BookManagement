using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookManagement.WPF.Services.Transactions;

public sealed class FavoriteService : IFavoriteService
{
    private readonly ProjectPrnContext _context;

    public FavoriteService(ProjectPrnContext context) => _context = context;

    public async Task<bool> AddAsync(string readerId, string bookId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        if (await _context.Favorites.AnyAsync(f => f.ReaderId == readerId && f.BookId == bookId, cancellationToken))
            return false;
        if (!await _context.Readers.AnyAsync(r => r.ReaderId == readerId, cancellationToken))
            throw new InvalidOperationException("Không tìm thấy Reader.");
        if (!await _context.Books.AnyAsync(b => b.BookId == bookId && b.Status == true, cancellationToken))
            throw new InvalidOperationException("Chỉ có thể yêu thích sách đã được duyệt.");

        _context.Favorites.Add(new Favorite { ReaderId = readerId, BookId = bookId, AddedAt = DateTime.UtcNow });
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveAsync(string readerId, string bookId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

        var favorite = await _context.Favorites.FindAsync([readerId, bookId], cancellationToken);
        if (favorite is null) return false;
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Favorite>> GetAllAsync(string readerId, CancellationToken cancellationToken = default) =>
        await _context.Favorites.AsNoTracking().Include(f => f.Book)
            .Where(f => f.ReaderId == readerId && f.Book.Status == true)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync(cancellationToken);
}
