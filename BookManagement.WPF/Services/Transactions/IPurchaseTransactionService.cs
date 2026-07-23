using BookManagement.WPF.Entities;

namespace BookManagement.WPF.Services.Transactions;

public interface IPurchaseTransactionService
{
    Task<Purchase> PurchaseAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<bool> AddToCartAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromCartAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetCartAsync(string readerId, CancellationToken cancellationToken = default);
    Task<bool> UpdateCartQuantityAsync(string readerId, string bookId, int newQuantity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> CheckoutAsync(string readerId, CancellationToken cancellationToken = default);
    Task<bool> CanDownloadAsync(string readerId, string bookId, string downloadToken, CancellationToken cancellationToken = default);
    //Task<string> GetAuthorizedFilePathAsync(string readerId, string bookId, string downloadToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetHistoryAsync(string readerId, CancellationToken cancellationToken = default);
}
