using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Author
{
    public partial class AuthorBookDetailView : UserControl
    {
        private readonly IBookService _bookService;
        private readonly BookModel _book;

        public AuthorBookDetailView(BookModel book)
        {
            InitializeComponent();

            _book = book;
            _bookService = App.Current.Services.GetRequiredService<IBookService>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_book != null)
            {
                txtBookId.Text = _book.Id;
                txtCreatedAt.Text = _book.SubmittedDate;
                txtTitle.Text = _book.Title;
                txtPrice.Text = _book.Price.ToString("F2");
                txtDescription.Text = _book.Description;

                foreach (ComboBoxItem item in cbCategory.Items)
                {
                    if (item.Content?.ToString() == _book.Category)
                    {
                        cbCategory.SelectedItem = item;
                        break;
                    }
                }
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
                // Update ONLY: Title, Description, Category, Price
                _book.Title = title;
                _book.Description = description;
                _book.Category = category;
                _book.Price = price;

                _bookService.UpdateBook(_book);

                MessageBox.Show("Cập nhật thông tin tác phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Navigate back
                BookManagement.Services.Navigation.NavigationService.Instance.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật tác phẩm: {ex.Message}", "Database Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            BookManagement.Services.Navigation.NavigationService.Instance.GoBack();
        }
    }
}
