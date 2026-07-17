using BookManagement.WPF.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public interface IFavoriteService
{
    Task<Favorite> AddFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task<bool> IsFavoriteAsync(
        string readerId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorite>> GetFavoritesAsync(
        string readerId,
        CancellationToken cancellationToken = default);
}
