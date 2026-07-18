using BookManagement.Models.Entities;
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
using System.Windows.Input;
using System.Windows.Navigation;
using BookManagement.Services.Utils;

using NavigationService =
BookManagement.Services.Navigation.NavigationService;
namespace BookManagement.Views.Reader
{
    public partial class ReaderHomeView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IPurchaseTransactionService _purchaseTransactionService;

        public ReaderHomeView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _purchaseTransactionService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();

            Loaded += ReaderHomeView_Loaded;
        }

        private async void ReaderHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user != null)
            {
                txtWelcome.Text = $"Welcome back, {user.FullName}!";
            }

            try
            {
                // 1. Load books from database
                var approvedBooks = _bookService.GetApprovedBooks().ToList();
                
                // Popular: Top 4 books sorted by Rating (we can seed a rating or fallback to a standard sorting)
                var popular = approvedBooks.OrderByDescending(b => b.Rating).Take(4).ToList();
                icPopularBooks.ItemsSource = popular;

                // Recommended: Top 4 books of same/different categories
                var recommended = approvedBooks.OrderBy(b => b.Id).Take(4).ToList();
                icRecommendedBooks.ItemsSource = recommended;

                // 2. Load categories
                var categories = new List<string> { "Tất cả" };
                categories.AddRange(approvedBooks.Select(b => b.Category).Distinct());
                cbCategory.ItemsSource = categories;
                cbCategory.SelectedIndex = 0;

                // 3. Load recent purchases from DB
                if (user != null)
                {
                    var history = await _purchaseTransactionService.GetHistoryAsync(user.AccountId);
                    var recent = history.OrderByDescending(p => p.PurchasedAt).Take(5).Select(p => new RecentlyPurchasedItemModel
                    {
                        BookId = p.BookId,
                        BookTitle= p.Book?.Title ?? "Unknown Title",
                        PurchaseDate = p.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                        Price = (double)p.Book.Price,
                        Status = p.IsBought ? "Completed" : "Failed",
                        CoverImagePath = "/Assets/Covers/placeholder.jpg"
                    }).ToList();
                    dgRecentPurchases.ItemsSource = recent;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp dữ liệu trang chủ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var query = txtSearch.Text;
            var category = cbCategory.SelectedItem?.ToString();
            if (category == "Tất cả") category = string.Empty;

            var booksView = new ReaderBooksView(query, category);
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(booksView);  //loi o day
            }
        }

        private void BookCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement card && card.DataContext is BookModel clickedBook)
            {
                var detailView = new ReaderBookDetailView(clickedBook);
                var nav = NavigationService.GetNavigationService();
                if (nav != null)
                {
                    nav.NavigateContent(detailView); //loi o day
                }
            }
        }

        private void BtnViewAllBooks_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(new ReaderBooksView());
            }
        }

        private void BtnViewAllLibrary_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(new ReaderLibraryView());
            }
        }

        private void BtnViewProfile_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(new ReaderProfileView());
            }
        }
    }
}
