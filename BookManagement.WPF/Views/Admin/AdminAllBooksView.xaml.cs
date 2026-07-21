using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.Views.Author;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminAllBooksView : UserControl
    {
        private readonly IBookService _bookService;
        private List<BookModel> _allBooks = new List<BookModel>();
        private bool _isInitializing = true;

        public AdminAllBooksView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;

            RefreshData();
            InitializeFilters();

            _isInitializing = false;
            ApplyFilters();
        }

        private void RefreshData()
        {
            try
            {
                _allBooks = _bookService.GetAllBooks().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeFilters()
        {
            // Category filter setup
            var categories = new List<string> { "Tất cả thể loại" };
            categories.AddRange(_allBooks.Select(b => b.Category).Distinct().OrderBy(c => c));
            cbCategory.ItemsSource = categories;
            cbCategory.SelectedIndex = 0;

            // Status filter setup
            var statuses = new List<string> { "Tất cả trạng thái", "Approved", "Pending", "Rejected" };
            cbStatus.ItemsSource = statuses;
            cbStatus.SelectedIndex = 0;
        }

        private void FilterInput_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string query = txtSearch.Text.Trim().ToLower();
            string categoryFilter = cbCategory.SelectedItem?.ToString() ?? "Tất cả thể loại";
            string statusFilter = cbStatus.SelectedItem?.ToString() ?? "Tất cả trạng thái";

            var filtered = _allBooks.Where(b =>
            {
                bool matchQuery = string.IsNullOrEmpty(query) || 
                                   b.Title.ToLower().Contains(query) || 
                                   b.Author.ToLower().Contains(query) ||
                                   b.Category.ToLower().Contains(query);

                bool matchCategory = categoryFilter == "Tất cả thể loại" || b.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase);

                bool matchStatus = statusFilter == "Tất cả trạng thái" || b.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);

                return matchQuery && matchCategory && matchStatus;
            }).ToList();

            dgBooks.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var nav = BookManagement.Services.Navigation.NavigationService.Instance;
                nav.NavigateContent(new AuthorBookDetailView(book, BookDetailMode.AdminReview));
            }
        }

        //private void BtnEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button btn && btn.DataContext is BookModel book)
        //    {
        //        var nav = BookManagement.Services.Navigation.NavigationService.Instance;
        //        nav.NavigateContent(new AuthorBookDetailView(book, BookDetailMode.AdminReview));
        //    }
        //}

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tác phẩm \"{book.Title}\"? Hành động này không thể hoàn tác.", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning); 
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bookService.DeleteBook(book.Id);
                        MessageBox.Show("Đã xóa tác phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        RefreshData();
                        ApplyFilters();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Xóa sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
