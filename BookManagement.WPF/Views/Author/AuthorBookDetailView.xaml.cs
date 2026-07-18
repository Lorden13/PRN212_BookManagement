using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using NavigationService =
BookManagement.Services.Navigation.NavigationService;
namespace BookManagement.Views.Author
{
    public partial class AuthorBookDetailView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly BookModel _book;

        public AuthorBookDetailView(BookModel book)
        {
            InitializeComponent();

            _book = book;
            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();

            Loaded += AuthorBookDetailView_Loaded;
        }

        private void AuthorBookDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBookDetails();
        }

        private void LoadBookDetails()
        {
            if (_book == null) return;

            txtCategory.Content = _book.Category;
            txtStatus.Content = _book.Status;
            txtTitle.Text = _book.Title;
            txtAuthor.Text = $"Tác giả: {_book.Author}";
            txtRating.Text = $"{_book.Rating:F1} / 5.0";
            txtPrice.Text = $"${_book.Price:F2}";
            txtDescription.Text = _book.Description;

            try
            {
                var reviews = _reviewService.GetReviewsByBookId(_book.Id).ToList();
                icReviews.ItemsSource = reviews;
                emptyReviewsState.Visibility = reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp lịch sử duyệt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                nav.NavigateContent(new AuthorBooksView());
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(new AuthorCreateBookView(_book));
            }
        }
    }
}
