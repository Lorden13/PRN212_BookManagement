using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public sealed class PurchaseTransactionService : IPurchaseTransactionService
{
    private readonly Func<ProjectPrnContext> _contextFactory;

    public PurchaseTransactionService()
        : this(() => new ProjectPrnContext())
    {
    }

    public PurchaseTransactionService(Func<ProjectPrnContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Purchase> PurchaseBookAsync(
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

        Reader? reader = await context.Readers
            .Include(item => item.ReaderNavigation)
            .SingleOrDefaultAsync(item => item.ReaderId == readerId, cancellationToken);

        if (reader is null || !reader.ReaderNavigation.IsActive)
        {
            throw new BusinessRuleException("Reader does not exist or the account is inactive.");
        }

        Book? book = await context.Books
            .SingleOrDefaultAsync(item => item.BookId == bookId, cancellationToken);

        if (book is null || book.Status is not true)
        {
            throw new BusinessRuleException("Only an approved book can be purchased.");
        }

        if (book.Price < 0)
        {
            throw new BusinessRuleException("Book price cannot be negative.");
        }

        Purchase? existingPurchase = await context.Purchases
            .SingleOrDefaultAsync(
                item => item.ReaderId == readerId && item.BookId == bookId,
                cancellationToken);

        if (existingPurchase is not null)
        {
            if (!existingPurchase.IsBought)
            {
                existingPurchase.IsBought = true;
                existingPurchase.Payment = book.Price;
                existingPurchase.DownloadToken = CreateDownloadToken();
                existingPurchase.PurchasedAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return existingPurchase;
        }

        Purchase purchase = new()
        {
            PurchaseId = Guid.NewGuid().ToString(),
            ReaderId = readerId,
            BookId = bookId,
            DownloadToken = CreateDownloadToken(),
            Payment = book.Price,
            IsBought = true,
            PurchasedAt = DateTime.UtcNow
        };

        await context.Purchases.AddAsync(purchase, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return purchase;
    }

    public async Task<bool> HasPurchasedAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));
        RequireId(bookId, nameof(bookId));

        await using ProjectPrnContext context = _contextFactory();
        return await context.Purchases
            .AsNoTracking()
            .AnyAsync(
                purchase => purchase.ReaderId == readerId &&
                            purchase.BookId == bookId &&
                            purchase.IsBought,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetReaderPurchasesAsync(
        string readerId,
        CancellationToken cancellationToken = default)
    {
        RequireId(readerId, nameof(readerId));

        await using ProjectPrnContext context = _contextFactory();
        return await PurchaseQuery(context)
            .Where(purchase => purchase.ReaderId == readerId && purchase.IsBought)
            .OrderByDescending(purchase => purchase.PurchasedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetAllPurchasesAsync(
        CancellationToken cancellationToken = default)
    {
        await using ProjectPrnContext context = _contextFactory();
        return await PurchaseQuery(context)
            .OrderByDescending(purchase => purchase.PurchasedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<DownloadAuthorizationResult> AuthorizeDownloadAsync(
        string readerId,
        string downloadToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(readerId) || string.IsNullOrWhiteSpace(downloadToken))
        {
            return DownloadAuthorizationResult.Denied("Reader ID and download token are required.");
        }

        await using ProjectPrnContext context = _contextFactory();
        Purchase? purchase = await context.Purchases
            .AsNoTracking()
            .Include(item => item.Reader)
            .ThenInclude(reader => reader.ReaderNavigation)
            .Include(item => item.Book)
            .SingleOrDefaultAsync(
                item => item.ReaderId == readerId && item.DownloadToken == downloadToken,
                cancellationToken);

        if (purchase is null)
        {
            return DownloadAuthorizationResult.Denied("The download token is invalid.");
        }

        if (!purchase.IsBought)
        {
            return DownloadAuthorizationResult.Denied("This purchase is not completed.");
        }

        if (!purchase.Reader.ReaderNavigation.IsActive)
        {
            return DownloadAuthorizationResult.Denied("The reader account is inactive.");
        }

        if (purchase.Book.Status is not true)
        {
            return DownloadAuthorizationResult.Denied("The book is no longer available for download.");
        }

        if (string.IsNullOrWhiteSpace(purchase.Book.FilePath))
        {
            return DownloadAuthorizationResult.Denied("The book file is not available.");
        }

        return new DownloadAuthorizationResult(
            true,
            "Download authorized.",
            purchase.BookId,
            purchase.Book.Title,
            purchase.Book.FilePath,
            purchase.Payment,
            purchase.PurchasedAt);
    }

    private static IQueryable<Purchase> PurchaseQuery(ProjectPrnContext context) =>
        context.Purchases
            .AsNoTracking()
            .Include(purchase => purchase.Book)
            .ThenInclude(book => book.Author)
            .ThenInclude(author => author.AuthorNavigation)
            .Include(purchase => purchase.Reader)
            .ThenInclude(reader => reader.ReaderNavigation);

    private static string CreateDownloadToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static void RequireId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty ID is required.", parameterName);
        }
    }
}
