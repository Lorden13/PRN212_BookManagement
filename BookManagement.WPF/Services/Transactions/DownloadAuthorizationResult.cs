using System;

namespace BookManagement.WPF.Services.Transactions;

public sealed record DownloadAuthorizationResult(
    bool IsAuthorized,
    string Message,
    string? BookId = null,
    string? BookTitle = null,
    string? FilePath = null,
    decimal? Payment = null,
    DateTime? PurchasedAt = null)
{
    public static DownloadAuthorizationResult Denied(string message) => new(false, message);
}
