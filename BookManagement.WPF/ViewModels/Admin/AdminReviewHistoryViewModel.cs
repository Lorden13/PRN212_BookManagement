using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminReviewHistoryViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IReviewService _reviewService;

        private string _searchText = string.Empty;
        private string _selectedResult = "All";

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

        public string SelectedResult
        {
            get => _selectedResult;
            set
            {
                if (SetProperty(ref _selectedResult, value))
                {
                    ReviewsView.Refresh();
                }
            }
        }

        public ObservableCollection<ReviewModel> Reviews { get; } = new ObservableCollection<ReviewModel>();
        public ICollectionView ReviewsView { get; }

        public ObservableCollection<string> Results { get; } = new ObservableCollection<string> { "All", "Approved", "Rejected" };

        public AdminReviewHistoryViewModel(DashboardViewModelBase dashboard, IReviewService reviewService)
        {
            _dashboard = dashboard;
            _reviewService = reviewService;

            ReviewsView = CollectionViewSource.GetDefaultView(Reviews);
            ReviewsView.Filter = FilterReviews;

            LoadReviews();
        }

        private void LoadReviews()
        {
            Reviews.Clear();
            var list = _reviewService.GetAllReviews().ToList();
            foreach (var r in list)
            {
                Reviews.Add(r);
            }
        }

        private bool FilterReviews(object item)
        {
            if (!(item is ReviewModel review)) return false;

            if (!string.IsNullOrEmpty(SearchText))
            {
                bool matchesSearch = review.BookTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                     review.AuthorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                if (!matchesSearch) return false;
            }

            if (SelectedResult != "All")
            {
                if (review.Result != SelectedResult) return false;
            }

            return true;
        }
    }
}
