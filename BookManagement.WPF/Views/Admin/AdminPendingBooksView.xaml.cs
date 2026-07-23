using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.Views.Author;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using NavigationService = BookManagement.Services.Navigation.NavigationService;

namespace BookManagement.Views.Admin
{
    public partial class AdminPendingBooksView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private List<BookModel> _allPendingBooks = new List<BookModel>();
        private bool _isInitializing = true;

        public AdminPendingBooksView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();

            Loaded += AdminPendingBooksView_Loaded;
        }

        private void AdminPendingBooksView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
            InitializeFilters();
            _isInitializing = false;
            ApplyFilters();
        }

        private void RefreshData()
        {
            try
            {
                _allPendingBooks = _bookService.GetPendingBooks().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp sách chờ duyệt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeFilters()
        {
            var authors = new List<string> { "Tất cả tác giả" };
            authors.AddRange(_allPendingBooks.Select(b => b.Author).Distinct());
            cbAuthor.ItemsSource = authors;
            cbAuthor.SelectedIndex = 0;
        }

        private void FilterInput_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string query = txtSearch.Text.Trim().ToLower();
            string authorFilter = cbAuthor.SelectedItem?.ToString() ?? "Tất cả tác giả";

            var filtered = _allPendingBooks.Where(b =>
            {
                bool matchQuery = string.IsNullOrEmpty(query) || 
                                   b.Title.ToLower().Contains(query) || 
                                   b.Category.ToLower().Contains(query);

                bool matchAuthor = authorFilter == "Tất cả tác giả" || b.Author.Equals(authorFilter, StringComparison.OrdinalIgnoreCase);

                return matchQuery && matchAuthor;
            }).ToList();

            dgPendingBooks.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var nav = NavigationService.Instance;
                nav.NavigateContent(new AdminBookDetailView(book, typeof(AdminPendingBooksView)));
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                try
                {
                    _bookService.ApproveBook(book.Id);

                    var review = new ReviewModel
                    {
                        BookId = book.Id,
                        Result = "Approved",
                        AdminComment = "Phê duyệt thông qua kiểm duyệt nhanh."
                    };
                    _reviewService.SubmitReview(review);

                    MessageBox.Show($"Đã phê duyệt cuốn sách \"{book.Title}\" thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshData();
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Phê duyệt thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                try
                {
                    _bookService.RejectBook(book.Id, "Từ chối kiểm duyệt");

                    var review = new ReviewModel
                    {
                        BookId = book.Id,
                        Result = "Rejected",
                        AdminComment = "Tác phẩm chưa đạt yêu cầu kiểm duyệt chất lượng nội dung hoặc định dạng bản thảo."
                    };
                    _reviewService.SubmitReview(review);

                    MessageBox.Show($"Đã từ chối cuốn sách \"{book.Title}\".", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    RefreshData();
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Từ chối thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
