using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Transactions;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
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
    public partial class ReaderBookDetailView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IPurchaseTransactionService _purchaseService;
        private readonly IFavoriteService _favoriteService;
        private readonly IReviewService _reviewService;

        private BookModel _book;

        public ReaderBookDetailView(BookModel book)
        {
            InitializeComponent();

            _book = book;

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _purchaseService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
            _favoriteService = App.Current.Services.GetRequiredService<IFavoriteService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();

            Loaded += ReaderBookDetailView_Loaded;
        }

        private void ReaderBookDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBookDetails();
        }

        private void LoadBookDetails()
        {
            if (_book == null) return;

            txtCategory.Content = _book.Category;
            txtTitle.Text = _book.Title;
            txtAuthor.Text = $"Tác giả: {_book.Author}";
            txtRating.Text = $"{_book.Rating:F1} / 5.0";
            txtPrice.Text = $"${_book.Price:F2}";
            txtDescription.Text = _book.Description;

            try
            {
                // Load reviews
                var reviews = _reviewService.GetReviewsByBookId(_book.Id).ToList();
                icReviews.ItemsSource = reviews;
                emptyReviewsState.Visibility = reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                // Load recommendations
                var recommendations = _bookService.GetApprovedBooks()
                    .Where(b => b.Id != _book.Id && b.Category == _book.Category)
                    .Take(3)
                    .ToList();

                // If not enough, fill from other categories
                if (recommendations.Count < 3)
                {
                    var extra = _bookService.GetApprovedBooks()
                        .Where(b => b.Id != _book.Id && b.Category != _book.Category)
                        .Take(3 - recommendations.Count);
                    recommendations.AddRange(extra);
                }

                icRecommendedBooks.ItemsSource = recommendations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp thông tin chi tiết: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null && nav.CanGoBack())
            {
                nav.GoBack();
            }
            else if (nav != null)
            {
                nav.NavigateContent(new ReaderBooksView());
            }
        }

        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                await _purchaseService.PurchaseAsync(user.AccountId, _book.Id);
                MessageBox.Show($"Đã mua sách thành công: {_book.Title}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mua sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                await _favoriteService.AddAsync(user.AccountId, _book.Id);
                MessageBox.Show($"Đã thêm {_book.Title} vào danh sách yêu thích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm yêu thích: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecommendedBookCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement card && card.DataContext is BookModel clickedBook)
            {
                _book = clickedBook;
                LoadBookDetails();
            }
        }
    }
}
