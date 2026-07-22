using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Transactions;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Services.Utils;





namespace BookManagement.Views.Reader
{
    public partial class ReaderFavoriteView : UserControl
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IPurchaseTransactionService _purchaseTransactionService;
        private readonly IBookService _bookService;
        private List<BookModel> _allFavorites = new List<BookModel>();

        public ReaderFavoriteView()
        {
            InitializeComponent();

            _favoriteService = App.Current.Services.GetRequiredService<IFavoriteService>();
            _purchaseTransactionService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += ReaderFavoriteView_Loaded;
        }

        private async void ReaderFavoriteView_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }

        private async System.Threading.Tasks.Task RefreshData()
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                var favoritesList = await _favoriteService.GetAllAsync(user.AccountId);
                _allFavorites = favoritesList.Select(f => _bookService.GetBookById(f.BookId)).Where(b => b != null).ToList();

                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách yêu thích: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearchFilter()
        {
            string query = txtSearch.Text.Trim().ToLower();
            var filtered = _allFavorites.Where(b =>
                string.IsNullOrEmpty(query) || 
                b.Title.ToLower().Contains(query) || 
                b.Author.ToLower().Contains(query)
            ).ToList();

            dgFavorites.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var user = UserSession.CurrentUser;
                if (user == null) return;

                var answer = MessageBox.Show(
                    $"Bạn muốn mua sách {book.Title}?",
                    "Xác nhận mua sách",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question);
                if (answer != MessageBoxResult.OK) return;

                try
                {
                    await _purchaseTransactionService.PurchaseAsync(user.AccountId, book.Id);
                    MessageBox.Show($"Đã mua sách thành công: {book.Title}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Mua sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var user = UserSession.CurrentUser;
                if (user == null) return;

                try
                {
                    await _favoriteService.RemoveAsync(user.AccountId, book.Id);
                    MessageBox.Show($"Đã xóa {book.Title} khỏi danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xóa yêu thích thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
