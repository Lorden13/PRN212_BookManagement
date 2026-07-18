using BookManagement.Models.DTOs;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Transactions;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Models.DTOs;
using BookManagement.Services.Utils;

namespace BookManagement.Views.Reader
{
    public partial class ReaderLibraryView : UserControl
    {
        private readonly IPurchaseTransactionService _purchaseTransactionService;
        private readonly IBookService _bookService;
        private List<RecentlyPurchasedItemModel> _allPurchases = new List<RecentlyPurchasedItemModel>();

        public ReaderLibraryView()
        {
            InitializeComponent();

            _purchaseTransactionService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += ReaderLibraryView_Loaded;
        }

        private async void ReaderLibraryView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                var history = await _purchaseTransactionService.GetHistoryAsync(user.AccountId);
                _allPurchases = history.Select(p => new RecentlyPurchasedItemModel
                {
                    BookId = p.BookId,
                    BookTitle = p.Book?.Title ?? "Unknown Title",
                    Author = p.Book?.Author?.AuthorNavigation?.FullName ?? "Unknown Author",
                    PurchaseDate = p.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                    Price = (double)p.Book.Price,
                    Status = p.IsBought ? "Completed" : "Failed",
                    CoverImagePath = "/Assets/Covers/placeholder.jpg"
                }).ToList();

                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp tủ sách cá nhân: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearchFilter()
        {
            string query = txtSearch.Text.Trim().ToLower();
            var filtered = _allPurchases.Where(p =>
                string.IsNullOrEmpty(query) || 
                p.BookTitle.ToLower().Contains(query) || 
                p.Author.ToLower().Contains(query) ||
                p.Status.ToLower().Contains(query)
            ).ToList();

            dgLibrary.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RecentlyPurchasedItemModel item)
            {
                MessageBox.Show($"Đang mở tài liệu đọc sách: \"{item.BookTitle}\". Trực quan hóa PDF đang được kết nối...", "Mở sách", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
