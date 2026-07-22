using BookManagement.Models.Entities;
using BookManagement.Services.Navigation;
using BookManagement.Services.Repository;
using BookManagement.Services.Utils;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.WPF.Views.Reader
{
    public partial class PurchaseHistoryView : UserControl
    {
        private readonly IPurchaseTransactionService _purchaseService;
        private readonly IBookService _bookService;

        public PurchaseHistoryView()
        {
            InitializeComponent();
            _purchaseService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
            _bookService = App.Current.Services.GetRequiredService<IBookService>();
            Loaded += PurchaseHistoryView_Loaded;
        }

        private async void PurchaseHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user is null) return;

            try
            {
                var purchases = await _purchaseService.GetHistoryAsync(user.AccountId);
                dgPurchasedBooks.ItemsSource = purchases;
                emptyPurchasedBooks.Visibility = purchases.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                dgPurchasedBooks.Visibility = purchases.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải sách đã mua: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnViewBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: Purchase purchase }) return;

            BookModel book = _bookService.GetBookById(purchase.BookId);
            if (book is not null)
            {
                NavigationService.Instance.NavigateContent(new BookManagement.Views.Reader.ReaderBookDetailView(book, isReadOnly: true));
            }
        }
    }
}
