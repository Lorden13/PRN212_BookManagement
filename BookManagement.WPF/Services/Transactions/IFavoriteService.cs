using BookManagement.WPF.Entities;

namespace BookManagement.WPF.Services.Transactions;

public interface IFavoriteService
{
    Task<bool> AddAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(string readerId, string bookId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Favorite>> GetAllAsync(string readerId, CancellationToken cancellationToken = default);
}
