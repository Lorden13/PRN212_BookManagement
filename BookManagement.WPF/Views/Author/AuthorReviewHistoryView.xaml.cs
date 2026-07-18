using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Services.Utils;

namespace BookManagement.Views.Author
{
    public partial class AuthorReviewHistoryView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private List<ReviewModel> _allReviews = new List<ReviewModel>();

        public AuthorReviewHistoryView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();

            Loaded += AuthorReviewHistoryView_Loaded;
        }

        private void AuthorReviewHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                var myBooks = _bookService.GetMyBooks(user.AccountId).ToList();
                var combinedReviews = new List<ReviewModel>();

                foreach (var book in myBooks)
                {
                    var reviews = _reviewService.GetReviewsByBookId(book.Id).ToList();
                    foreach (var r in reviews)
                    {
                        r.BookTitle = book.Title;
                        r.AuthorName = book.Author;
                    }
                    combinedReviews.AddRange(reviews);
                }

                _allReviews = combinedReviews.OrderByDescending(r => r.Date).ToList();
                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp lịch sử đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearchFilter()
        {
            string query = txtSearch.Text.Trim().ToLower();
            var filtered = _allReviews.Where(r =>
                string.IsNullOrEmpty(query) || 
                r.BookTitle.ToLower().Contains(query) || 
                r.Result.ToLower().Contains(query) ||
                r.AdminComment.ToLower().Contains(query)
            ).ToList();

            dgReviews.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }
    }
}
