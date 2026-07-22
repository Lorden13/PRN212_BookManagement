using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagement.WPF.Services.Transactions
{
    public class ReviewService : IReviewService
    {
        private readonly ProjectPrnContext _dbContext;

        public ReviewService()
        {
            _dbContext = new ProjectPrnContext();
        }

        private ReviewModel MapToModel(BookApproval approval)
        {
            string adminName = approval.Admin?.AdminNavigation?.FullName ?? "Administrator";
            string bookTitle = approval.Book?.Title ?? "Unknown Book";

            return new ReviewModel
            {
                Id = approval.ApprovalId,
                BookId = approval.BookId,
                BookTitle = bookTitle,
                AuthorName = adminName,
                Result = approval.IsApproved ? "Approved" : "Rejected",
                AdminComment = approval.Feedback,
                Date = approval.ActionDate.ToString("yyyy-MM-dd HH:mm")
            };
        }

        public IEnumerable<ReviewModel> GetReviewsByBookId(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return Enumerable.Empty<ReviewModel>();

            var approvals = _dbContext.BookApprovals
                .Include(a => a.Book)
                .Include(a => a.Admin)
                .ThenInclude(ad => ad.AdminNavigation)
                .Where(a => a.BookId == bookId)
                .ToList();
            return approvals.Select(MapToModel).ToList();
        }

        public IEnumerable<ReviewModel> GetAllReviews()
        {
            var approvals = _dbContext.BookApprovals
                .Include(a => a.Book)
                .Include(a => a.Admin)
                .ThenInclude(ad => ad.AdminNavigation)
                .ToList();
            return approvals.Select(MapToModel).ToList();
        }

        public void SubmitReview(ReviewModel review)
        {
            if (review == null) return;

            string adminId = "c1916a5c-7dba-489d-b88c-a48d1738a765"; // Default fallback (seeded Admin AccountID)
            if (BookManagement.Services.Utils.UserSession.CurrentUser != null)
            {
                adminId = BookManagement.Services.Utils.UserSession.CurrentUser.AccountId;
                // Ensure this Account is registered as an Admin in the Admin table to prevent FK constraint error
                var isAdmin = _dbContext.Admins.Any(a => a.AdminId == adminId);
                if (!isAdmin)
                {
                    _dbContext.Admins.Add(new Admin { AdminId = adminId });
                    _dbContext.SaveChanges();
                }
            }

            var approval = new BookApproval
            {
                ApprovalId = Guid.NewGuid().ToString(),
                BookId = review.BookId,
                AdminId = adminId,
                IsApproved = review.Result.Equals("Approved", StringComparison.OrdinalIgnoreCase),
                Feedback = review.AdminComment,
                ActionDate = DateTime.Now
            };

            _dbContext.BookApprovals.Add(approval);
            _dbContext.SaveChanges();

            review.Id = approval.ApprovalId;
        }
    }

    public sealed class ReaderReviewService : IReaderReviewService
    {
        private readonly ProjectPrnContext _dbContext;

        public ReaderReviewService(ProjectPrnContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ReaderReviewModel>> GetByBookIdAsync(
            string bookId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

            var reviews = await _dbContext.ReaderReviews
                .AsNoTracking()
                .Include(r => r.Reader)
                .ThenInclude(r => r.ReaderNavigation)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new ReaderReviewModel
                {
                    Id = r.ReviewId,
                    BookId = r.BookId,
                    ReaderId = r.ReaderId,
                    ReaderName = r.Reader.ReaderNavigation.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Date = r.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                })
                .ToList();
        }

        public async Task<double> GetAverageRatingAsync(
            string bookId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(bookId);

            return await _dbContext.ReaderReviews
                .AsNoTracking()
                .Where(r => r.BookId == bookId)
                .Select(r => (double?)r.Rating)
                .AverageAsync(cancellationToken) ?? 0;
        }

        public async Task SubmitAsync(
            string readerId,
            string bookId,
            int rating,
            string comment,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookId);
            if (rating is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Điểm đánh giá phải từ 1 đến 5.");
            if (comment.Length > 1000)
                throw new ArgumentException("Nội dung đánh giá không được vượt quá 1000 ký tự.", nameof(comment));

            if (!await _dbContext.Readers.AnyAsync(r => r.ReaderId == readerId, cancellationToken))
                throw new InvalidOperationException("Không tìm thấy độc giả.");
            if (!await _dbContext.Books.AnyAsync(b => b.BookId == bookId && b.Status == true, cancellationToken))
                throw new InvalidOperationException("Chỉ có thể đánh giá sách đã được duyệt.");

            var existing = await _dbContext.ReaderReviews.SingleOrDefaultAsync(
                r => r.ReaderId == readerId && r.BookId == bookId,
                cancellationToken);

            if (existing is null)
            {
                _dbContext.ReaderReviews.Add(new ReaderReview
                {
                    ReviewId = Guid.NewGuid().ToString(),
                    ReaderId = readerId,
                    BookId = bookId,
                    Rating = rating,
                    Comment = comment.Trim(),
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Rating = rating;
                existing.Comment = comment.Trim();
                existing.CreatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
