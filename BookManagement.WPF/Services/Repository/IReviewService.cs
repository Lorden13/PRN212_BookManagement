using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IReviewService
    {
        IEnumerable<ReviewModel> GetReviewsByBookId(string bookId);
        IEnumerable<ReviewModel> GetAllReviews();
        void SubmitReview(ReviewModel review);
    }

    public interface IReaderReviewService
    {
        Task<IReadOnlyList<ReaderReviewModel>> GetByBookIdAsync(string bookId, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(string bookId, CancellationToken cancellationToken = default);
        Task SubmitAsync(string readerId, string bookId, int rating, string comment, CancellationToken cancellationToken = default);
    }
}
