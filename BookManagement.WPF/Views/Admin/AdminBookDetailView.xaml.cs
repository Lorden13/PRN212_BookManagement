using BookManagement.Models.Entities;
using BookManagement.Services.Navigation;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminBookDetailView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly BookModel _book;
        private readonly Type _returnViewType;

        public AdminBookDetailView(BookModel book) : this(book, null)
        {
        }

        public AdminBookDetailView(BookModel book, Type returnViewType)
        {
            InitializeComponent();

            _book = book;
            _returnViewType = returnViewType;
            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            LoadBookDetails();
            LoadReviewHistory();
        }

        private void LoadBookDetails()
        {
            txtBookId.Text = _book.Id ?? string.Empty;
            txtCreatedAt.Text = _book.SubmittedDate ?? string.Empty;
            txtAuthor.Text = _book.Author ?? string.Empty;
            txtStatus.Text = _book.Status ?? "Pending";
            txtTitle.Text = _book.Title ?? string.Empty;
            txtCategory.Text = _book.Category ?? string.Empty;
            txtPrice.Text = _book.Price.ToString("F2");
            txtStock.Text = _book.Stock.ToString();
            txtDescription.Text = _book.Description ?? string.Empty;

            // Hide Approve/Reject buttons if book is already Approved
            bool isApproved = string.Equals(_book.Status, "Approved", StringComparison.OrdinalIgnoreCase);
            panelFeedback.Visibility = isApproved ? Visibility.Collapsed : Visibility.Visible;
            btnApprove.Visibility = isApproved ? Visibility.Collapsed : Visibility.Visible;
            btnReject.Visibility = isApproved ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadReviewHistory()
        {
            try
            {
                var history = _reviewService.GetReviewsByBookId(_book.Id).ToList();
                lstReviewHistory.ItemsSource = history;
                panelReviewHistory.Visibility = history.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lịch sử kiểm duyệt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateBack()
        {
            var nav = NavigationService.Instance;
            if (nav.CanGoBack())
            {
                nav.GoBack();
            }
            else if (_returnViewType != null)
            {
                try
                {
                    var targetView = Activator.CreateInstance(_returnViewType);
                    nav.NavigateContent(targetView);
                }
                catch
                {
                    nav.NavigateContent(new AdminPendingBooksView());
                }
            }
            else
            {
                nav.NavigateContent(new AdminPendingBooksView());
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            string comment = txtFeedback.Text.Trim();
            if (string.IsNullOrEmpty(comment)) comment = "Phê duyệt thông qua kiểm duyệt.";

            try
            {
                _bookService.ApproveBook(_book.Id);

                var review = new ReviewModel
                {
                    BookId = _book.Id,
                    Result = "Approved",
                    AdminComment = comment
                };
                _reviewService.SubmitReview(review);

                MessageBox.Show($"Đã phê duyệt cuốn sách \"{_book.Title}\" thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Phê duyệt thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            string comment = txtFeedback.Text.Trim();
            if (string.IsNullOrEmpty(comment))
            {
                MessageBox.Show("Vui lòng nhập lý do từ chối kiểm duyệt.", "Yêu cầu phản hồi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _bookService.RejectBook(_book.Id, comment);

                var review = new ReviewModel
                {
                    BookId = _book.Id,
                    Result = "Rejected",
                    AdminComment = comment
                };
                _reviewService.SubmitReview(review);

                MessageBox.Show($"Đã từ chối cuốn sách \"{_book.Title}\".", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Từ chối thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
