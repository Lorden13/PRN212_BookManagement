using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using NavigationService =
BookManagement.Services.Navigation.NavigationService;
namespace BookManagement.Views.Admin
{
    public partial class AdminHomeView : UserControl
    {
        private readonly IBookService _bookService;

        public AdminHomeView()
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += AdminHomeView_Loaded;
        }

        private void AdminHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new ProjectPrnContext())
                {
                    // 1. Fetch count stats
                    int totalUsers = db.Accounts.Count();
                    int totalReaders = db.Readers.Count();
                    int totalAuthors = db.Authors.Count();
                    int totalBooks = db.Books.Count();
                    int pendingBooksCount = db.Books.Count(b => b.Status == null);
                    decimal totalRevenue = db.Purchases.Where(p => p.IsBought).Sum(p => (decimal?)p.Book.Price) ?? 0;

                    DateTime today = DateTime.Today;
                    decimal todaySales = db.Purchases.Where(p => p.IsBought && p.PurchasedAt >= today).Sum(p => (decimal?)p.Book.Price) ?? 0;

                    // Update UI cards
                    cardTotalUsers.Value = totalUsers.ToString();
                    cardTotalReaders.Value = totalReaders.ToString();
                    cardTotalAuthors.Value = totalAuthors.ToString();
                    cardTotalBooks.Value = totalBooks.ToString();
                    cardPendingBooks.Value = pendingBooksCount.ToString();
                    cardTotalRevenue.Value = $"${totalRevenue:F2}";
                    cardTodaySales.Value = $"${todaySales:F2}";

                    // 2. Compute status percentages
                    int approvedCount = db.Books.Count(b => b.Status == true);
                    int rejectedCount = db.Books.Count(b => b.Status == false);

                    int totalStatusCount = approvedCount + pendingBooksCount + rejectedCount;
                    int appPct = totalStatusCount > 0 ? (approvedCount * 100 / totalStatusCount) : 0;
                    int penPct = totalStatusCount > 0 ? (pendingBooksCount * 100 / totalStatusCount) : 0;
                    int rejPct = totalStatusCount > 0 ? (rejectedCount * 100 / totalStatusCount) : 0;

                    pbApprovedStatus.Value = appPct;
                    lblApprovedPercent.Text = $"{appPct}%";

                    pbPendingStatus.Value = penPct;
                    lblPendingPercent.Text = $"{penPct}%";

                    pbRejectedStatus.Value = rejPct;
                    lblRejectedPercent.Text = $"{rejPct}%";

                    // 3. Load activities log dynamically
                    var activities = new List<dynamic>();

                    // Fetch approvals
                    //var approvals = db.BookApprovals.OrderByDescending(ba => ba.ReviewedAt).Take(5).ToList();
                    //foreach (var a in approvals)
                    //{
                    //    var b = db.Books.FirstOrDefault(x => x.BookId == a.BookId);
                    //    string title = b?.Title ?? "Tác phẩm ẩn";
                    //    string res = a.Status == true ? "Đã duyệt" : "Từ chối";
                    //    activities.Add(new
                    //    {
                    //        Message = $"Ấn phẩm \"{title}\" được {res.ToLower()} bởi Ban kiểm duyệt.",
                    //        Time = a.ReviewedAt.ToString("yyyy-MM-dd HH:mm"),
                    //        Timestamp = a.ReviewedAt
                    //    });
                    //}

                    // Fetch purchases
                    var purchases = db.Purchases.Where(p => p.IsBought).OrderByDescending(p => p.PurchasedAt).Take(5).ToList();
                    foreach (var p in purchases)
                    {
                        var b = db.Books.FirstOrDefault(x => x.BookId == p.BookId);
                        var r = db.Accounts.FirstOrDefault(x => x.AccountId == p.ReaderId);
                        string rName = r?.FullName ?? "Độc giả";
                        string title = b?.Title ?? "Sách";
                        activities.Add(new
                        {
                            Message = $"Độc giả \"{rName}\" đã mua thành công cuốn sách \"{title}\".",
                            Time = p.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                            Timestamp = p.PurchasedAt
                        });
                    }

                    icActivities.ItemsSource = activities.OrderByDescending(a => a.Timestamp).Take(8).ToList();
                }

                // 4. Load pending books
                var pendingBooks = _bookService.GetPendingBooks().Take(5).ToList();
                dgPendingBooks.ItemsSource = pendingBooks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp thống kê trang chủ quản trị: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReviewBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookModel book)
            {
                var nav = NavigationService.GetNavigationService(); //loi o day
                if (nav != null)
                {
                    nav.NavigateContent(new AdminBookReviewView(book));  //loi ow day 
                }
            }
        }
    }
}
