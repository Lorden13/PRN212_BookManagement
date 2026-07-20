using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BookManagement.Services.Utils;
using NavigationService =
BookManagement.Services.Navigation.NavigationService;


namespace BookManagement.Views.Author
{
    public partial class AuthorHomeView : UserControl
    {
        private readonly IBookService _bookService;

        public AuthorHomeView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += AuthorHomeView_Loaded;
        }

        private void AuthorHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            try
            {
                var myBooks = _bookService.GetMyBooks(user.AccountId).ToList();

                int total = myBooks.Count;
                int approved = myBooks.Count(b => b.Status == "Approved");
                int pending = myBooks.Count(b => b.Status == "Pending");
                int rejected = myBooks.Count(b => b.Status == "Rejected");

                // Update statistic cards
                cardTotal.Value = total.ToString();
                cardApproved.Value = approved.ToString();
                cardPending.Value = pending.ToString();
                cardRejected.Value = rejected.ToString();

                // Update progress bars
                int maxVal = total > 0 ? total : 100;
                pbApproved.Maximum = maxVal;
                pbApproved.Value = approved;
                lblApproved.Text = $"{approved} cuốn";

                pbPending.Maximum = maxVal;
                pbPending.Value = pending;
                lblPending.Text = $"{pending} cuốn";

                pbRejected.Maximum = maxVal;
                pbRejected.Value = rejected;
                lblRejected.Text = $"{rejected} cuốn";

                // Bind DataGrid
                var recent = myBooks.Take(5).ToList();
                dgRecentBooks.ItemsSource = recent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp thống kê trang chủ tác giả: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCreateBook_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null)
            {
                nav.NavigateContent(new AuthorCreateBookView());
            }
        }

        private void BtnViewBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var nav = NavigationService.GetNavigationService();
                if (nav != null)
                {
                    nav.NavigateContent (new AuthorBookDetailView(book));
                }
            }
        }
    }
}
