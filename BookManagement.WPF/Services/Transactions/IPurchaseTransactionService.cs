using BookManagement.WPF.Entities;

namespace BookManagement.WPF.Services.Transactions;

public interface IPurchaseTransactionService
{
    Task<Purchase> PurchaseAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<bool> CanDownloadAsync(string readerId, string bookId, string downloadToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetHistoryAsync(string readerId, CancellationToken cancellationToken = default);
}
