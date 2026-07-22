using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.Views.Author;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BookManagement.Services.Repository;
namespace BookManagement.Views.Author
{
    public enum BookDetailMode
    {
        AuthorEdit,
        AdminReview
    }

    public partial class AuthorBookDetailView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly BookModel _book;
        private readonly BookDetailMode _mode;

        public AuthorBookDetailView(BookModel book) : this(book, BookDetailMode.AuthorEdit)
        {
        }

        public AuthorBookDetailView(BookModel book, BookDetailMode mode)
        {
            InitializeComponent();

            _book = book;
            _mode = mode;
            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _reviewService = App.Current.Services.GetRequiredService<IReviewService>();
        }


        private void LoadBook(BookModel book)
        {
            txtBookId.Text = book.Id;
            txtCreatedAt.Text = book.SubmittedDate;
            txtAuthor.Text = book.Author;
            txtStatus.Text = book.Status;

            txtTitle.Text = book.Title;
            txtPrice.Text = book.Price.ToString("F2");
            txtDescription.Text = book.Description;

            foreach (ComboBoxItem item in cbCategory.Items)
            {
                if (item.Content?.ToString() == book.Category)
                {
                    cbCategory.SelectedItem = item;
                    break;
                }
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (_book == null) return;

            _originalBook = new BookModel
            {
                Id = _book.Id,
                Title = _book.Title,
                Description = _book.Description,
                Category = _book.Category,
                Price = _book.Price,
                Author = _book.Author,
                SubmittedDate = _book.SubmittedDate,
                Status = _book.Status
            };

            LoadBook(_book);

            LoadReviewHistory();
            ApplyModeConfiguration();
    //        _dbContext.Books
    //.Where(b => b.Status == true)
    //.ToList();

           // _bookService.GetBookById(_book.Id);
            
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

        private void ApplyModeConfiguration()
        {
            var disabledBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"));
            var enabledBrush = new SolidColorBrush(Colors.White);

            if (_mode == BookDetailMode.AdminReview)
            {
                txtSectionTitle.Text = "Kiểm duyệt chi tiết tác phẩm";

                // Disable all form fields
                txtTitle.IsReadOnly = true;
                txtTitle.Background = disabledBrush;

                cbCategory.IsEnabled = false;

                txtPrice.IsReadOnly = true;
                txtPrice.Background = disabledBrush;

                txtDescription.IsReadOnly = true;
                txtDescription.Background = disabledBrush;

                // Show Admin review sections
                panelFeedback.Visibility = Visibility.Visible;
                panelAdminButtons.Visibility = Visibility.Visible;
                panelAuthorButtons.Visibility = Visibility.Collapsed;
            }
            else // AuthorEdit mode
            {
                txtSectionTitle.Text = "Chỉnh sửa chi tiết tác phẩm";

                // Enable form fields
                txtTitle.IsReadOnly = false;
                txtTitle.Background = enabledBrush;

                cbCategory.IsEnabled = true;

                txtPrice.IsReadOnly = false;
                txtPrice.Background = enabledBrush;

                txtDescription.IsReadOnly = false;
                txtDescription.Background = enabledBrush;

                // Hide Admin review sections
                panelFeedback.Visibility = Visibility.Collapsed;
                panelAdminButtons.Visibility = Visibility.Collapsed;
                panelAuthorButtons.Visibility = Visibility.Visible;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            string title = txtTitle.Text.Trim();
            string category = (cbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            string priceText = txtPrice.Text.Trim();
            string description = txtDescription.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Tiêu đề tác phẩm là bắt buộc.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Mô tả tóm tắt là bắt buộc.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Thể loại là bắt buộc.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(priceText, out double price) || price <= 0)
            {
                MessageBox.Show("Giá bán phải là số thực lớn hơn 0.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _book.Title = title;
                _book.Description = description;
                _book.Category = category;
                _book.Price = price;

                _bookService.UpdateBook(_book);

                MessageBox.Show("Cập nhật thông tin tác phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                BookManagement.Services.Navigation.NavigationService.Instance.NavigateContent(new AuthorBooksView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật tác phẩm: {ex.Message}", "Database Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BookModel _originalBook;

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var nav = BookManagement.Services.Navigation.NavigationService.Instance;
            if (nav.CanGoBack())
            {
                nav.GoBack();
            }
            else
            {
                nav.NavigateContent(new AuthorBooksView());
            }
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
                BookManagement.Services.Navigation.NavigationService.Instance.GoBack();
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

                MessageBox.Show($"Đã từ chối cuốn sách \"{_book.Title}\".", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                BookManagement.Services.Navigation.NavigationService.Instance.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Từ chối thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdminDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tác phẩm \"{_book.Title}\"? Hành động này không thể hoàn tác.", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bookService.DeleteBook(_book.Id);
                    MessageBox.Show("Đã xóa tác phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    BookManagement.Services.Navigation.NavigationService.Instance.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xóa sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            NavigationService.Instance.NavigateContent(new AuthorBooksView());
        }
    }
}
