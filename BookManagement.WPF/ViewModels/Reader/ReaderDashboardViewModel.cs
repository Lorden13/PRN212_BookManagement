using BookManagement.Helpers;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderDashboardViewModel : DashboardViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IPurchaseService _purchaseService;
        private readonly IReviewService _reviewService;
        private readonly IReaderService _readerService;
        private readonly IUserService _userService;

        public ReaderDashboardViewModel(
            IUserService userService, 
            IBookService bookService,
            IPurchaseService purchaseService,
            IReviewService reviewService,
            IReaderService readerService,
            INotificationService notificationService) 
            : base(userService.AuthenticateDemo("Reader"), notificationService)
        {
            _userService = userService;
            _bookService = bookService;
            _purchaseService = purchaseService;
            _reviewService = reviewService;
            _readerService = readerService;

            PageTitle = "Home";
            InitializeMenu();

            // Set default sub-page
            CurrentPageViewModel = new ReaderHomeViewModel(this, _bookService, _purchaseService);
        }

        private void InitializeMenu()
        {
            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Home",
                IconGeometryKey = "IconHome",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Home";
                    CurrentPageViewModel = new ReaderHomeViewModel(this, _bookService, _purchaseService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Books",
                IconGeometryKey = "IconBooks",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Books";
                    CurrentPageViewModel = new ReaderBooksViewModel(this, _bookService, _purchaseService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "My Library",
                IconGeometryKey = "IconBook",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "My Library";
                    CurrentPageViewModel = new ReaderLibraryViewModel(this, _purchaseService, _bookService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Profile",
                IconGeometryKey = "IconUser",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Profile";
                    CurrentPageViewModel = new ReaderProfileViewModel(this, _readerService);
                })
            }));

            Sidebar.MenuItems.Add(new MenuItemViewModel(new MenuItemModel
            {
                Title = "Change Password",
                IconGeometryKey = "IconKey",
                Command = new RelayCommand(() => 
                {
                    PageTitle = "Change Password";
                    CurrentPageViewModel = new ChangePasswordViewModel(this, _userService);
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
