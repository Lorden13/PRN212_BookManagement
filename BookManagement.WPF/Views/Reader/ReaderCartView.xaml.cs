using BookManagement.Services.Utils;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Reader;

public partial class ReaderCartView : UserControl
{
    private readonly IPurchaseTransactionService _purchaseService;
    private IReadOnlyList<Purchase> _cartItems = Array.Empty<Purchase>();

    public ReaderCartView()
    {
        InitializeComponent();
        _purchaseService = App.Current.Services.GetRequiredService<IPurchaseTransactionService>();
        Loaded += ReaderCartView_Loaded;
    }

    private async void ReaderCartView_Loaded(object sender, RoutedEventArgs e)
    {
        await RefreshCartAsync();
    }

    private async Task RefreshCartAsync()
    {
        var user = UserSession.CurrentUser;
        if (user is null) return;

        try
        {
            _cartItems = await _purchaseService.GetCartAsync(user.AccountId);
            dgCart.ItemsSource = _cartItems;
            txtCartSummary.Text = $"{_cartItems.Count} sách trong giỏ hàng";
            txtTotal.Text = $"${_cartItems.Sum(item => item.Book.Price):F2}";
            emptyState.Visibility = _cartItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            btnCheckout.Visibility = _cartItems.Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnRemove_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: Purchase item }) return;
        var user = UserSession.CurrentUser;
        if (user is null) return;

        try
        {
            await _purchaseService.RemoveFromCartAsync(user.AccountId, item.BookId);
            await RefreshCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể xóa sách khỏi giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnCheckout_Click(object sender, RoutedEventArgs e)
    {
        var user = UserSession.CurrentUser;
        if (user is null || _cartItems.Count < 2) return;

        var total = _cartItems.Sum(item => item.Book.Price);
        var answer = MessageBox.Show(
            $"Bạn muốn thanh toán {_cartItems.Count} sách với tổng tiền ${total:F2}?",
            "Xác nhận thanh toán",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Question);
        if (answer != MessageBoxResult.OK) return;

        try
        {
            await _purchaseService.CheckoutAsync(user.AccountId);
            MessageBox.Show("Thanh toán thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            await RefreshCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Thanh toán thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
