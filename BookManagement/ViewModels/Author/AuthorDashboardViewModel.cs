using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorDashboardViewModel : DashboardViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IAuthorService _authorService;
        private readonly IUserService _userService;

        public AuthorDashboardViewModel(
            IUserService userService, 
            IBookService bookService,
            IReviewService reviewService,
            IAuthorService authorService,
            INotificationService notificationService) 
            : base(userService.AuthenticateDemo("Author"), notificationService)
        {
            _userService = userService;
            _bookService = bookService;
            _reviewService = reviewService;
            _authorService = authorService;

            PageTitle = "Dashboard";
            InitializeMenu();

            // Set default sub-page
            CurrentPageViewModel = new AuthorHomeViewModel(this, _bookService);
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
                    CurrentPageViewModel = new AuthorHomeViewModel(this, _bookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "My Books",
                IconGeometryKey = "IconBooks",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "My Books";
                    CurrentPageViewModel = new AuthorBooksViewModel(this, _bookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Create Book",
                IconGeometryKey = "IconPlus",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Create Book";
                    CurrentPageViewModel = new AuthorCreateBookViewModel(this, _bookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Review History",
                IconGeometryKey = "IconClock",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Review History";
                    CurrentPageViewModel = new AuthorReviewHistoryViewModel(this, _reviewService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Approval Status",
                IconGeometryKey = "IconShield",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Approval Status";
                    CurrentPageViewModel = new AuthorApprovalStatusViewModel(this, _bookService, _reviewService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Profile",
                IconGeometryKey = "IconUser",
                Command = new RelayCommand(() =>
                {
                    PageTitle = "Profile";
                    CurrentPageViewModel = new AuthorProfileViewModel(this, _authorService);
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
