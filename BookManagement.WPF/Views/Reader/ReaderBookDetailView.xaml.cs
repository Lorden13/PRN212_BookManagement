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
        private readonly IReaderReviewService _reviewService;

        private BookModel _book;
        private readonly bool _isReadOnly;

        public ReaderBookDetailView(BookModel book, bool isReadOnly = false)
        {
            InitializeComponent();

            _book = book;
            _isReadOnly = isReadOnly;
            actionButtonsPanel.Visibility = isReadOnly ? Visibility.Collapsed : Visibility.Visible;
            txtStock.Visibility = isReadOnly ? Visibility.Collapsed : Visibility.Visible;

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _purchaseService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
            _favoriteService = App.Current.Services.GetRequiredService<IFavoriteService>();
            _reviewService = App.Current.Services.GetRequiredService<IReaderReviewService>();

            Loaded += ReaderBookDetailView_Loaded;
        }

        private async void ReaderBookDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBookDetailsAsync();
        }

        private async Task LoadBookDetailsAsync()
        {
            if (_book == null) return;

            txtCategory.Content = _book.Category;
            txtTitle.Text = _book.Title;
            txtAuthor.Text = $"Tác giả: {_book.Author}";
            txtRating.Text = $"{_book.Rating:F1} / 5.0";
            txtPrice.Text = $"${_book.Price:F2}";
            txtDescription.Text = _book.Description;

            // Stock display
            if (_isReadOnly)
            {
                txtStock.Visibility = Visibility.Collapsed;
            }
            else if (_book.Stock > 0)
            {
                txtStock.Visibility = Visibility.Visible;
                txtStock.Text = $"Còn {_book.Stock} cuốn";
                txtStock.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#16A34A"));
            }
            else
            {
                txtStock.Visibility = Visibility.Visible;
                txtStock.Text = "Hết hàng";
                txtStock.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626"));
            }

            //// Disable buy/cart when out of stock
            //if (!_isReadOnly)
            //{
            //    btnBuy.IsEnabled = _book.Stock > 0;
            //    btnAddToCart.IsEnabled = _book.Stock > 0;
            //}

            try
            {
                // Load reviews
                var reviews = await _reviewService.GetByBookIdAsync(_book.Id);
                icReviews.ItemsSource = reviews;
                emptyReviewsState.Visibility = reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                _book.Rating = reviews.Count == 0
                    ? 0
                    : Math.Round(reviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero);
                txtRating.Text = $"{_book.Rating:F1} / 5.0";

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
            if (_isReadOnly)
            {
                nav?.NavigateContent(new BookManagement.WPF.Views.Reader.PurchaseHistoryView());
            }
            else if (nav != null && nav.CanGoBack())
            {
                nav.GoBack();
            }
            else if (nav != null)
            {
                nav.NavigateContent(new ReaderBooksView());
            }
        }

        //private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        //{
        //    var user = UserSession.CurrentUser;
        //    if (user == null) return;

        //    var answer = MessageBox.Show(
        //        $"Bạn muốn mua sách {_book.Title}?",
        //        "Xác nhận mua sách",
        //        MessageBoxButton.OKCancel,
        //        MessageBoxImage.Question);
        //    if (answer != MessageBoxResult.OK) return;

        //    try
        //    {
        //        await _purchaseService.PurchaseAsync(user.AccountId, _book.Id);
        //        MessageBox.Show($"Đã mua sách thành công: {_book.Title}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Mua sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private async void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user is null) return;

            try
            {
                await _purchaseService.AddToCartAsync(user.AccountId, _book.Id);
                MessageBox.Show(
                    $"Đã thêm \"{_book.Title}\" vào giỏ hàng thành công.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm vào giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnShowReview_Click(object sender, RoutedEventArgs e)
        {
            reviewEditor.Visibility = Visibility.Visible;
        }

        private void BtnCancelReview_Click(object sender, RoutedEventArgs e)
        {
            reviewEditor.Visibility = Visibility.Collapsed;
        }

        private async void BtnSubmitReview_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user is null || cbReviewRating.SelectedItem is not ComboBoxItem selectedRating) return;

            try
            {
                var rating = int.Parse(selectedRating.Tag.ToString()!);
                await _reviewService.SubmitAsync(user.AccountId, _book.Id, rating, txtReviewComment.Text);
                MessageBox.Show("Đánh giá của bạn đã được lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                reviewEditor.Visibility = Visibility.Collapsed;
                txtReviewComment.Clear();
                await LoadBookDetailsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void RecommendedBookCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BookManagement.Controls.BookCard.IsActionButtonClick(e)) return;
            if (sender is FrameworkElement card && card.DataContext is BookModel clickedBook)
            {
                _book = clickedBook;
                reviewEditor.Visibility = Visibility.Collapsed;
                await LoadBookDetailsAsync();
            }
        }
    }
}
