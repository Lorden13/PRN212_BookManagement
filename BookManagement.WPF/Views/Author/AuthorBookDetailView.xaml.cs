using BookManagement.Models.Entities;
using BookManagement.Services.Navigation;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            LoadBook(_book);
            LoadReviewHistory();
        }

        private void LoadBook(BookModel book)
        {
            txtBookId.Text = book.Id ?? string.Empty;
            txtCreatedAt.Text = book.SubmittedDate ?? string.Empty;
            txtAuthor.Text = book.Author ?? string.Empty;
            txtStatus.Text = book.Status ?? "Pending";

            txtTitle.Text = book.Title ?? string.Empty;
            txtPrice.Text = book.Price.ToString("F2");
            txtStock.Text = book.Stock.ToString();
            txtDescription.Text = book.Description ?? string.Empty;

            foreach (ComboBoxItem item in cbCategory.Items)
            {
                if (item.Content?.ToString() == book.Category)
                {
                    cbCategory.SelectedItem = item;
                    break;
                }
            }
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_book == null) return;

            string title = txtTitle.Text.Trim();
            string category = (cbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            string priceText = txtPrice.Text.Trim();
            string stockText = txtStock.Text.Trim();
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

            if (!int.TryParse(stockText, out int stock) || stock < 0)
            {
                MessageBox.Show("Số lượng tồn kho phải là số nguyên lớn hơn hoặc bằng 0.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _book.Title = title;
                _book.Description = description;
                _book.Category = category;
                _book.Price = price;
                _book.Stock = stock;

                _bookService.UpdateBook(_book);

                MessageBox.Show("Cập nhật thông tin tác phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Instance.NavigateContent(new AuthorBooksView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật tác phẩm: {ex.Message}", "Lỗi lưu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContent(new AuthorBooksView());
        }
    }
}
