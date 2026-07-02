using System;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminDashboardViewModel : DashboardViewModelBase
    {
        public IUserService UserService { get; }
        public IBookService BookService { get; }
        public IReaderService ReaderService { get; }
        public IAuthorService AuthorService { get; }
        public IPurchaseService PurchaseService { get; }
        public IReviewService ReviewService { get; }

        public AdminDashboardViewModel(
            IUserService userService,
            IBookService bookService,
            IReaderService readerService,
            IAuthorService authorService,
            IPurchaseService purchaseService,
            IReviewService reviewService,
            INotificationService notificationService) 
            : base(userService.AuthenticateDemo("Admin"), notificationService)
        {
            UserService = userService;
            BookService = bookService;
            ReaderService = readerService;
            AuthorService = authorService;
            PurchaseService = purchaseService;
            ReviewService = reviewService;

            PageTitle = "Dashboard";
            InitializeMenu();

            // Set default sub-page
            CurrentPageViewModel = new AdminHomeViewModel(this, BookService, ReaderService, AuthorService, PurchaseService, ReviewService);
        }

        private void InitializeMenu()
        {
            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Dashboard",
                IconGeometryKey = "IconHome",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Dashboard";
                    CurrentPageViewModel = new AdminHomeViewModel(this, BookService, ReaderService, AuthorService, PurchaseService, ReviewService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "User Management",
                IconGeometryKey = "IconUsers",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "User Management";
                    CurrentPageViewModel = new AdminUsersViewModel(this, ReaderService, AuthorService, PurchaseService, BookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Pending Books",
                IconGeometryKey = "IconShield",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Pending Books";
                    CurrentPageViewModel = new AdminPendingBooksViewModel(this, BookService, AuthorService, ReviewService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "All Books",
                IconGeometryKey = "IconBooks",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "All Books";
                    CurrentPageViewModel = new AdminAllBooksViewModel(this, BookService, ReviewService, AuthorService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Purchases",
                IconGeometryKey = "IconCart",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Purchases";
                    CurrentPageViewModel = new AdminPurchasesViewModel(this, PurchaseService, BookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Review History",
                IconGeometryKey = "IconClock",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Review History";
                    CurrentPageViewModel = new AdminReviewHistoryViewModel(this, ReviewService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Reports",
                IconGeometryKey = "IconChart",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Reports";
                    CurrentPageViewModel = new AdminReportsViewModel(this, PurchaseService, ReaderService, AuthorService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Profile",
                IconGeometryKey = "IconUser",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Profile";
                    CurrentPageViewModel = new AdminProfileViewModel(this, UserService);
                })
            }));

            // Set initial selected item
            if (Sidebar.MenuItems.Count > 0)
            {
                Sidebar.MenuItems[0].IsSelected = true;
                Sidebar.SelectedItem = Sidebar.MenuItems[0];
            }
        }
    }
}
