using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IReviewService
    {
        IEnumerable<ReviewModel> GetReviewsByBookId(int bookId);
        IEnumerable<ReviewModel> GetAllReviews();
        void SubmitReview(ReviewModel review);
    }
}
