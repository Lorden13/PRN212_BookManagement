using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
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
    public partial class AdminReaderDetailView : UserControl
    {
        private readonly string _userId;

        public AdminReaderDetailView() : this(string.Empty)
        {
        }

        public AdminReaderDetailView(string userId)
        {
            InitializeComponent();

            _userId = userId;

            Loaded += AdminReaderDetailView_Loaded;
        }

        private void AdminReaderDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userId)) return;

            try
            {
                using (var db = new ProjectPrnContext())
                {
                    // 1. Fetch user general information
                    var user = db.Accounts
                        .Include(a => a.Role)
                        .FirstOrDefault(a => a.AccountId == _userId);

                    if (user == null)
                    {
                        MessageBox.Show("Không tìm thấy thông tin tài khoản.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    txtReaderNameHeader.Text = user.FullName;
                    if (!string.IsNullOrEmpty(user.FullName))
                    {
                        txtReaderNameLetter.Text = user.FullName[0].ToString().ToUpper();
                    }
                    txtReaderStatus.Content = user.IsActive ? "Active" : "Inactive";
                    txtReaderId.Text = $"#USR-{user.AccountId.Substring(0, 8).ToUpper()}";
                    txtReaderEmail.Text = user.Email;
                    txtReaderPhone.Text = user.Phone ?? "N/A";
                    txtReaderRole.Text = user.Role?.RoleName ?? "Reader";

                    // 2. Fetch spending metrics
                    var myPurchases = db.Purchases
                        .Include(p => p.Book)
                        .Where(p => p.ReaderId == _userId)
                        .ToList();

                    int totalPurchasesCount = myPurchases.Count(p => p.IsBought);
                    decimal totalSpendingAmount = myPurchases.Where(p => p.IsBought).Sum(p => p.Book.Price);

                    txtTotalPurchases.Text = $"{totalPurchasesCount} đơn hàng";
                    txtTotalSpending.Text = $"${totalSpendingAmount:F2}";

                    // 3. Bind Purchase History Grid
                    var purchaseHistoryList = myPurchases.Select(p => new
                    {
                        Id = $"#TXN-{p.PurchaseId.Substring(0, 8).ToUpper()}",
                        BookTitle = p.Book?.Title ?? "Unknown Title",
                        Price = (double)p.Book.Price,
                        PurchaseDate = p.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                        Status = p.IsBought ? "Completed" : "Failed"
                    }).ToList();
                    dgPurchases.ItemsSource = purchaseHistoryList;

                    // 4. Bind Favorites
                    var favoriteList = db.Favorites
                        .Include(f => f.Book)
                        .Where(f => f.ReaderId == _userId)
                        .ToList();

                    var favoriteBooks = favoriteList.Select(f => new BookModel
                    {
                        Id = f.Book.BookId,
                        Title = f.Book.Title,
                        Author = "Author", // Seed fallback or lookup
                        Category = f.Book.Category,
                        Price = (double)f.Book.Price,
                        Status = f.Book.Status == true ? "Approved" : "Pending",
                        CoverImagePath = "/Assets/Covers/placeholder.jpg"
                    }).ToList();
                    icFavorites.ItemsSource = favoriteBooks;

                    // 5. Bind Activities
                    var activities = new List<dynamic>();

                    foreach (var p in myPurchases)
                    {
                        activities.Add(new
                        {
                            Message = p.IsBought 
                                ? $"Đã mua thành công cuốn sách \"{p.Book?.Title ?? "Unknown Title"}\""
                                : $"Thanh toán thất bại cho cuốn sách \"{p.Book?.Title ?? "Unknown Title"}\"",
                            Time = p.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                            Timestamp = p.PurchasedAt
                        });
                    }

                    foreach (var f in favoriteList)
                    {
                        activities.Add(new
                        {
                            Message = $"Đã thêm cuốn sách \"{f.Book?.Title ?? "Unknown Title"}\" vào danh sách yêu thích.",
                            Time = "Gần đây",
                            Timestamp = DateTime.Now.AddDays(-1)
                        });
                    }

                    icActivities.ItemsSource = activities.OrderByDescending(a => a.Timestamp).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp thông tin chi tiết: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService();
            if (nav != null && nav.CanGoBack())
            {
                nav.GoBack();
            }
            else if (nav != null)
            {
                nav.NavigateContent(new AdminUsersView());
            }
        }
    }
}
