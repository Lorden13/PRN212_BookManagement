using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorReviewHistoryViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IReviewService _reviewService;
        private string _searchText = "";

        public ObservableCollection<ReviewModel> Reviews { get; } = new ObservableCollection<ReviewModel>();
        public ICollectionView ReviewsView { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ReviewsView.Refresh();
                }
            }
        }

        public AuthorReviewHistoryViewModel(DashboardViewModelBase dashboard, IReviewService reviewService)
        {
            _dashboard = dashboard;
            _reviewService = reviewService;

            LoadReviews();

            ReviewsView = CollectionViewSource.GetDefaultView(Reviews);
            ReviewsView.Filter = FilterReviews;
        }

        private void LoadReviews()
        {
            Reviews.Clear();
            // Filter reviews for Alice Johnson's books
            var authorReviews = _reviewService.GetAllReviews().Where(r => r.AuthorName == "Alice Johnson");
            foreach (var r in authorReviews)
            {
                Reviews.Add(r);
            }
        }

        private bool FilterReviews(object obj)
        {
            if (obj is ReviewModel review)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var text = SearchText.ToLower();
                    return review.BookTitle.ToLower().Contains(text) || 
                           review.Result.ToLower().Contains(text) ||
                           (review.AdminComment != null && review.AdminComment.ToLower().Contains(text));
                }
                return true;
            }
            return false;
        }
    }
}
