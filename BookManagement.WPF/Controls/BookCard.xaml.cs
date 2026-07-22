using BookManagement.Models.Entities;
using BookManagement.Services.Utils;
using BookManagement.Views.Reader;
using BookManagement.WPF.Services.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BookManagement.Controls
{
    public partial class BookCard : UserControl
    {
        public BookCard()
        {
            InitializeComponent();
        }

      

        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BookModel book)
            {
                var user = UserSession.CurrentUser;
                if (user is null) return;

                var answer = MessageBox.Show(
                    $"Bạn muốn mua sách {book.Title}?",
                    "Xác nhận mua sách",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question);
                if (answer != MessageBoxResult.OK) return;

                try
                {
                    var service = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
                    await service.PurchaseAsync(user.AccountId, book.Id);
                    MessageBox.Show($"Đã mua sách thành công: {book.Title}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Mua sách thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
        }

        public static bool IsActionButtonClick(MouseButtonEventArgs e)
        {
            var current = e.OriginalSource as DependencyObject;
            while (current is not null)
            {
                if (current is Button) return true;
                if (current is BookCard) return false;
                current = VisualTreeHelper.GetParent(current);
            }

            return false;
        }

        private async void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not BookModel book) return;
            var user = UserSession.CurrentUser;
            if (user is null) return;

            try
            {
                var service = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
                var added = await service.AddToCartAsync(user.AccountId, book.Id);
                MessageBox.Show(
                    added
                        ? $"Đã thêm {book.Title} vào giỏ hàng."
                        : $"{book.Title} đã có trong giỏ hàng hoặc đã được mua.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm vào giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }
    }
}
