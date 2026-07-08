using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Mock
{
    public class MockReviewService : IReviewService
    {
        private readonly List<ReviewModel> _reviews = new List<ReviewModel>();

        public MockReviewService()
        {
            // Seed 15 review records
            string[] dates = { "2026-05-22", "2026-05-20", "2026-05-18", "2026-05-15", "2026-05-12", "2026-05-10", "2026-05-08" };
            
            _reviews.Add(new ReviewModel { Id = 1, BookId = 19, BookTitle = "Python Data Science", AuthorName = "Alice Johnson", Result = "Rejected", AdminComment = "Please improve the content quality and examples.", Date = "2026-05-10 09:40" });
            _reviews.Add(new ReviewModel { Id = 2, BookId = 17, BookTitle = "ASP.NET Core Guide", AuthorName = "Alice Johnson", Result = "Approved", AdminComment = "Good job! Very complete and easy to understand.", Date = "2026-05-18 10:15" });
            _reviews.Add(new ReviewModel { Id = 3, BookId = 18, BookTitle = "Learn SQL Server", AuthorName = "Alice Johnson", Result = "Approved", AdminComment = "Clear content and good examples.", Date = "2026-05-15 16:20" });
            _reviews.Add(new ReviewModel { Id = 4, BookId = 20, BookTitle = "Web Design with HTML & CSS", AuthorName = "Alice Johnson", Result = "Approved", AdminComment = "Looks good.", Date = "2026-05-08 11:30" });
            _reviews.Add(new ReviewModel { Id = 5, BookId = 1, BookTitle = "Clean Code", AuthorName = "Robert C. Martin", Result = "Approved", AdminComment = "Excellent work.", Date = "2026-05-22" });

            for (int i = 6; i <= 15; i++)
            {
                _reviews.Add(new ReviewModel
                {
                    Id = i,
                    BookId = i,
                    BookTitle = "Mock Book " + i,
                    AuthorName = "Demo Author",
                    Result = i % 4 == 0 ? "Rejected" : "Approved",
                    AdminComment = i % 4 == 0 ? "Re-upload with better formatting." : "Approved by system administrator.",
                    Date = dates[i % dates.Length]
                });
            }
        }

        public IEnumerable<ReviewModel> GetReviewsByBookId(int bookId) => _reviews.Where(r => r.BookId == bookId);

        public IEnumerable<ReviewModel> GetAllReviews() => _reviews;

        public void SubmitReview(ReviewModel review)
        {
            review.Id = _reviews.Count + 1;
            _reviews.Add(review);
        }
    }
}
