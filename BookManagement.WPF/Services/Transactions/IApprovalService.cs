using BookManagement.WPF.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookManagement.WPF.Services.Transactions;

public interface IApprovalService
{
    Task<Book> SubmitBookAsync(string bookId, string authorId, CancellationToken cancellationToken = default);

    Task<BookApproval> ApproveBookAsync(
        string bookId,
        string adminId,
        string? feedback = null,
        CancellationToken cancellationToken = default);

    Task<BookApproval> RejectBookAsync(
        string bookId,
        string adminId,
        string feedback,
        CancellationToken cancellationToken = default);

    Task<BookApproval> UpdateFeedbackAsync(
        string approvalId,
        string adminId,
        string feedback,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Book>> GetPendingBooksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookApproval>> GetBookHistoryAsync(
        string bookId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookApproval>> GetAuthorHistoryAsync(
        string authorId,
        CancellationToken cancellationToken = default);
}
