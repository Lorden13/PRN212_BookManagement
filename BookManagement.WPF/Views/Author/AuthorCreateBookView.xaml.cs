using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Author
{
    public partial class AuthorCreateBookView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly BookModel? _editingBook;

        public AuthorCreateBookView() : this(null)
        {
        }

        public AuthorCreateBookView(BookModel? editingBook)
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            _editingBook = editingBook;

            Loaded += AuthorCreateBookView_Loaded;
        }

        private void AuthorCreateBookView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_editingBook != null)
            {
                txtSectionTitle.Text = "Cập nhật thông tin sách";
                btnSubmit.Content = "Cập nhật";

                txtTitle.Text = _editingBook.Title;
                txtPrice.Text = _editingBook.Price.ToString("F2");
                txtDescription.Text = _editingBook.Description;
                
                // Select category in ComboBox
                foreach (ComboBoxItem item in cbCategory.Items)
                {
                    if (item.Content?.ToString() == _editingBook.Category)
                    {
                        cbCategory.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                cbCategory.SelectedIndex = 0;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string category = (cbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Novel";
            string priceText = txtPrice.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề sách.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(priceText, out double price) || price < 0)
            {
                MessageBox.Show("Giá bán không hợp lệ. Vui lòng nhập số dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Vui lòng nhập mô tả tóm tắt sách.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = UserSession.CurrentUser;
                string authorName = user?.FullName ?? "Unknown Author";

                if (_editingBook == null)
                {
                    // Create mode
                    var newBook = new BookModel
                    {
                        Title = title,
                        Author = authorName,
                        Category = category,
                        Price = price,
                        Description = description,
                        FilePath = "Manuscripts/default_manuscript.pdf",
                        Status = "Pending"
                    };

                    _bookService.CreateBook(newBook);
                    MessageBox.Show("Đã gửi yêu cầu đăng ký sách thành công! Đang chờ ban quản trị phê duyệt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Edit mode
                    _editingBook.Title = title;
                    _editingBook.Category = category;
                    _editingBook.Price = price;
                    _editingBook.Description = description;
                    _editingBook.Status = "Pending"; // reset status to Pending upon editing

                    _bookService.UpdateBook(_editingBook);
                    MessageBox.Show("Cập nhật thông tin sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Thao tác thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (_editingBook != null)
            {
                AuthorCreateBookView_Loaded(this, new RoutedEventArgs());
            }
            else
            {
                txtTitle.Text = string.Empty;
                cbCategory.SelectedIndex = 0;
                txtPrice.Text = string.Empty;
                txtDescription.Text = string.Empty;
            }
        }

        private void NavigateBack()
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
    }
}
