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

}
