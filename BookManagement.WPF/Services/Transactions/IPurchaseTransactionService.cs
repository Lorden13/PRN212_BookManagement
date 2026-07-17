using BookManagement.WPF.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public interface IPurchaseTransactionService
{
    Task<Purchase> PurchaseBookAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task<bool> HasPurchasedAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Purchase>> GetReaderPurchasesAsync(
        string readerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Purchase>> GetAllPurchasesAsync(CancellationToken cancellationToken = default);

    Task<DownloadAuthorizationResult> AuthorizeDownloadAsync(
        string readerId,
        string downloadToken,
        CancellationToken cancellationToken = default);
}
