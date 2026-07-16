using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }

    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel = null!;
        private User? _currentUser;
        private bool _isLoggedIn;
        private string _currentPageTitle = string.Empty;
        private MenuItem? _selectedMenuItem;

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(_ => Logout());
            CurrentViewModel = new LoginViewModel(this);
        }

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (SetProperty(ref _currentUser, value))
                {
                    IsLoggedIn = value != null;
                    OnPropertyChanged(nameof(UserInitials));
                    OnPropertyChanged(nameof(UserFullName));
                    OnPropertyChanged(nameof(UserRoleDisplay));
                    UpdateMenuItems();
                }
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set => SetProperty(ref _currentPageTitle, value);
        }

        public MenuItem? SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        public string UserInitials => CurrentUser?.AvatarInitials ?? "";
        public string UserFullName => CurrentUser?.FullName ?? "";
        public string UserRoleDisplay => CurrentUser?.Role.ToString() ?? "";

        public ObservableCollection<MenuItem> MenuItems { get; } = new();

        public RelayCommand NavigateCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public void Login(User user)
        {
            CurrentUser = user;
            NavigateToDashboard();
        }

        public void Logout()
        {
            CurrentUser = null;
            MenuItems.Clear();
            CurrentPageTitle = "";
            SelectedMenuItem = null;
            CurrentViewModel = new LoginViewModel(this);
        }

        public void NavigateToDashboard()
        {
            if (CurrentUser == null) return;

            switch (CurrentUser.Role)
            {
                case UserRole.Admin:
                    NavigateTo("AdminDashboard");
                    break;
                case UserRole.Author:
                    NavigateTo("AuthorDashboard");
                    break;
                case UserRole.Reader:
                    NavigateTo("ReaderDashboard");
                    break;
            }
        }

        private void Navigate(object? parameter)
        {
            if (parameter is string tag)
            {
                NavigateTo(tag);
            }
        }

        public void NavigateTo(string tag)
        {
            switch (tag)
            {
                // Admin
                case "AdminDashboard":
                    CurrentViewModel = new AdminDashboardViewModel(this);
                    CurrentPageTitle = "Dashboard";
                    break;
                case "UserManagement":
                    CurrentViewModel = new UserManagementViewModel(this);
                    CurrentPageTitle = "User Management";
                    break;
                case "AdminBookManagement":
                    CurrentViewModel = new AdminBookManagementViewModel(this);
                    CurrentPageTitle = "Book Management";
                    break;
                case "PendingBooks":
                    CurrentViewModel = new PendingBooksViewModel(this);
                    CurrentPageTitle = "Pending Books";
                    break;
                case "ModerationHistory":
                    CurrentViewModel = new ModerationHistoryViewModel(this);
                    CurrentPageTitle = "Moderation History";
                    break;
                case "AdminPurchases":
                    CurrentViewModel = new AdminPurchasesViewModel(this);
                    CurrentPageTitle = "Purchases";
                    break;
                case "AdminProfile":
                    CurrentViewModel = new AdminProfileViewModel(this);
                    CurrentPageTitle = "My Profile";
                    break;

                // Author
                case "AuthorDashboard":
                    CurrentViewModel = new AuthorDashboardViewModel(this);
                    CurrentPageTitle = "Dashboard";
                    break;
                case "MyBooks":
                    CurrentViewModel = new MyBooksViewModel(this);
                    CurrentPageTitle = "My Books";
                    break;
                case "AddBook":
                    CurrentViewModel = new AddBookViewModel(this);
                    CurrentPageTitle = "Add Book";
                    break;
                case "EditBook":
                    CurrentViewModel = new EditBookViewModel(this);
                    CurrentPageTitle = "Edit Book";
                    break;
                case "ApprovalStatus":
                    CurrentViewModel = new ApprovalStatusViewModel(this);
                    CurrentPageTitle = "Approval Status";
                    break;
                case "ApprovalHistory":
                    CurrentViewModel = new ApprovalHistoryViewModel(this);
                    CurrentPageTitle = "Approval History";
                    break;

                // Reader
                case "ReaderDashboard":
                    CurrentViewModel = new ReaderDashboardViewModel(this);
                    CurrentPageTitle = "Dashboard";
                    break;
                case "BrowseBooks":
                    CurrentViewModel = new BrowseBooksViewModel(this);
                    CurrentPageTitle = "Browse Books";
                    break;
                case "BookDetails":
                    CurrentViewModel = new BookDetailsViewModel(this);
                    CurrentPageTitle = "Book Details";
                    break;
                case "FavoriteBooks":
                    CurrentViewModel = new FavoriteBooksViewModel(this);
                    CurrentPageTitle = "Favorite Books";
                    break;
                case "PurchaseHistory":
                    CurrentViewModel = new PurchaseHistoryViewModel(this);
                    CurrentPageTitle = "Purchase History";
                    break;
                case "Profile":
                    CurrentViewModel = new ProfileViewModel(this);
                    CurrentPageTitle = "My Profile";
                    break;
                case "ChangePassword":
                    CurrentViewModel = new ChangePasswordViewModel(this);
                    CurrentPageTitle = "Change Password";
                    break;

                // Auth
                case "Login":
                    CurrentViewModel = new LoginViewModel(this);
                    break;
                case "Register":
                    CurrentViewModel = new RegisterViewModel(this);
                    break;
                case "ForgotPassword":
                    CurrentViewModel = new ForgotPasswordViewModel(this);
                    break;
            }

            // Update selected menu item
            var menuItem = MenuItems.FirstOrDefault(m => m.Tag == tag);
            if (menuItem != null) SelectedMenuItem = menuItem;
        }

        public void NavigateToEditBook(Book book)
        {
            var vm = new EditBookViewModel(this, book);
            CurrentViewModel = vm;
            CurrentPageTitle = "Edit Book";
        }

        public void NavigateToBookDetails(Book book)
        {
            var vm = new BookDetailsViewModel(this, book);
            CurrentViewModel = vm;
            CurrentPageTitle = "Book Details";
        }

        private void UpdateMenuItems()
        {
            MenuItems.Clear();
            if (CurrentUser == null) return;

            switch (CurrentUser.Role)
            {
                case UserRole.Admin:
                    MenuItems.Add(new MenuItem { Title = "Dashboard", Icon = "📊", Tag = "AdminDashboard" });
                    MenuItems.Add(new MenuItem { Title = "User Management", Icon = "👥", Tag = "UserManagement" });
                    MenuItems.Add(new MenuItem { Title = "Book Management", Icon = "📚", Tag = "AdminBookManagement" });
                    MenuItems.Add(new MenuItem { Title = "Pending Books", Icon = "⏳", Tag = "PendingBooks" });
                    MenuItems.Add(new MenuItem { Title = "Moderation History", Icon = "📋", Tag = "ModerationHistory" });
                    MenuItems.Add(new MenuItem { Title = "Purchases", Icon = "💳", Tag = "AdminPurchases" });
                    MenuItems.Add(new MenuItem { Title = "Profile", Icon = "👤", Tag = "AdminProfile" });
                    break;
                case UserRole.Author:
                    MenuItems.Add(new MenuItem { Title = "Dashboard", Icon = "📊", Tag = "AuthorDashboard" });
                    MenuItems.Add(new MenuItem { Title = "My Books", Icon = "📖", Tag = "MyBooks" });
                    MenuItems.Add(new MenuItem { Title = "Add Book", Icon = "➕", Tag = "AddBook" });
                    MenuItems.Add(new MenuItem { Title = "Approval Status", Icon = "📋", Tag = "ApprovalStatus" });
                    MenuItems.Add(new MenuItem { Title = "Approval History", Icon = "📜", Tag = "ApprovalHistory" });
                    MenuItems.Add(new MenuItem { Title = "Profile", Icon = "👤", Tag = "Profile" });
                    break;
                case UserRole.Reader:
                    MenuItems.Add(new MenuItem { Title = "Dashboard", Icon = "📊", Tag = "ReaderDashboard" });
                    MenuItems.Add(new MenuItem { Title = "Browse Books", Icon = "🔍", Tag = "BrowseBooks" });
                    MenuItems.Add(new MenuItem { Title = "Favorite Books", Icon = "❤️", Tag = "FavoriteBooks" });
                    MenuItems.Add(new MenuItem { Title = "Purchase History", Icon = "🛒", Tag = "PurchaseHistory" });
                    MenuItems.Add(new MenuItem { Title = "Profile", Icon = "👤", Tag = "Profile" });
                    break;
            }
        }
    }
}
