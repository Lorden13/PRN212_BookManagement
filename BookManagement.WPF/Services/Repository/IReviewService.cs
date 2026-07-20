using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IReviewService
    {
        IEnumerable<ReviewModel> GetReviewsByBookId(string bookId);
        IEnumerable<ReviewModel> GetAllReviews();
        void SubmitReview(ReviewModel review);
    }
}
