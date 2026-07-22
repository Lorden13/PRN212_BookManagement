using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BookManagement.Services.Utils;

using NavigationService =
BookManagement.Services.Navigation.NavigationService;



namespace BookManagement.Views.Author
{
    public partial class AuthorBooksView : UserControl
    {
        private readonly IBookService _bookService;
        private List<BookModel> _allBooks = new List<BookModel>();
        private bool _isInitializing = true;

        public AuthorBooksView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += AuthorBooksView_Loaded;
        }

        private void AuthorBooksView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFilters();
            RefreshData();
            _isInitializing = false;
            ApplyFilters();
        }

        private void InitializeFilters()
        {
            cbStatus.ItemsSource = new List<string> { "Tất cả trạng thái", "Đang chờ duyệt", "Đã phê duyệt", "Bị từ chối" };
            cbStatus.SelectedIndex = 0;
        }

        private void RefreshData()
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                _allBooks = _bookService.GetMyBooks(user.AccountId).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void FilterInput_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string query = txtSearch.Text.Trim().ToLower();
            string statusFilter = cbStatus.SelectedItem?.ToString() ?? "Tất cả trạng thái";

            var filtered = _allBooks.Where(b =>
            {
                bool matchQuery = string.IsNullOrEmpty(query) || 
                                  b.Title.ToLower().Contains(query) || 
                                  b.Category.ToLower().Contains(query);

                bool matchStatus = true;
                if (statusFilter == "Đang chờ duyệt") matchStatus = b.Status == "Pending";
                else if (statusFilter == "Đã phê duyệt") matchStatus = b.Status == "Approved";
                else if (statusFilter == "Bị từ chối" || statusFilter == "Yêu cầu sửa đổi") matchStatus = b.Status == "Rejected";

                return matchQuery && matchStatus;
            }).ToList();

            dgBooks.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnCreateBook_Click(object sender, RoutedEventArgs e)
        {
            var nav = BookManagement.Services.Navigation.NavigationService.Instance;
            nav.NavigateContent(new AuthorCreateBookView());
        }

        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var currentUser = UserSession.CurrentUser;
                if (currentUser == null || currentUser.FullName != book.Author)
                {
                    MessageBox.Show("Bạn không có quyền chỉnh sửa tác phẩm của tác giả khác.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var nav = BookManagement.Services.Navigation.NavigationService.Instance;
                nav.NavigateContent(new AuthorBookDetailView(book));
            }
        }

        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var currentUser = UserSession.CurrentUser;
                if (currentUser == null || currentUser.FullName != book.Author)
                {
                    MessageBox.Show("Bạn không có quyền xóa tác phẩm của tác giả khác.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tác phẩm \"{book.Title}\"? Hành động này không thể hoàn tác.", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bookService.DeleteBook(book.Id);
                        MessageBox.Show("Đã xóa tác phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshData();
                        ApplyFilters();
                        NavigationService.Instance.NavigateContent(new AuthorBooksView());

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
