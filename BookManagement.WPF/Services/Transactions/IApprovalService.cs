using BookManagement.WPF.Entities;

namespace BookManagement.WPF.Services.Transactions;

public interface IApprovalService
{
    Task SubmitBookAsync(string bookId, string authorId, CancellationToken cancellationToken = default);
    Task<BookApproval> ApproveAsync(string bookId, string adminId, string? feedback = null, CancellationToken cancellationToken = default);
    Task<BookApproval> RejectAsync(string bookId, string adminId, string feedback, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookApproval>> GetHistoryAsync(string bookId, CancellationToken cancellationToken = default);
}
