using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public sealed class FavoriteService : IFavoriteService
{
    private readonly Func<ProjectPrnContext> _contextFactory;

    public FavoriteService()
        : this(() => new ProjectPrnContext())
    {
    }

    public FavoriteService(Func<ProjectPrnContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Favorite> AddFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));
        RequireId(bookId, nameof(bookId));

        await using ProjectPrnContext context = _contextFactory();
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        bool readerIsActive = await context.Readers
            .Where(reader => reader.ReaderId == readerId)
            .Select(reader => reader.ReaderNavigation.IsActive)
            .SingleOrDefaultAsync(cancellationToken);

        if (!readerIsActive)
        {
            throw new BusinessRuleException("Reader does not exist or the account is inactive.");
        }

        bool bookIsAvailable = await context.Books
            .AnyAsync(book => book.BookId == bookId && book.Status == true, cancellationToken);

        if (!bookIsAvailable)
        {
            throw new BusinessRuleException("Only an approved book can be added to favorites.");
        }

        Favorite? existing = await context.Favorites
            .SingleOrDefaultAsync(
                favorite => favorite.ReaderId == readerId && favorite.BookId == bookId,
                cancellationToken);

        if (existing is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            return existing;
        }

        Favorite favorite = new()
        {
            ReaderId = readerId,
            BookId = bookId,
            AddedAt = DateTime.UtcNow
        };

        await context.Favorites.AddAsync(favorite, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return favorite;
    }

    public async Task<bool> RemoveFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));
        RequireId(bookId, nameof(bookId));

        await using ProjectPrnContext context = _contextFactory();
        Favorite? favorite = await context.Favorites
            .SingleOrDefaultAsync(
                item => item.ReaderId == readerId && item.BookId == bookId,
                cancellationToken);

        if (favorite is null)
        {
            return false;
        }

        context.Favorites.Remove(favorite);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));
        RequireId(bookId, nameof(bookId));

        await using ProjectPrnContext context = _contextFactory();
        return await context.Favorites
            .AsNoTracking()
            .AnyAsync(
                favorite => favorite.ReaderId == readerId && favorite.BookId == bookId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Favorite>> GetFavoritesAsync(
        string readerId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));

        await using ProjectPrnContext context = _contextFactory();
        return await context.Favorites
            .AsNoTracking()
            .Where(favorite => favorite.ReaderId == readerId && favorite.Book.Status == true)
            .Include(favorite => favorite.Book)
            .ThenInclude(book => book.Author)
            .ThenInclude(author => author.AuthorNavigation)
            .OrderByDescending(favorite => favorite.AddedAt)
            .ToListAsync(cancellationToken);
    }

    private static void RequireId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty ID is required.", parameterName);
        }
    }
}
