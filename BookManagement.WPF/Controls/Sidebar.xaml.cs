using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Services.Navigation;
using BookManagement.ViewModels.Author;
using BookManagement.ViewModels.Reader;
using BookManagement.ViewModels.Admin;
using BookManagement.ViewModels.Common;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using BookManagement.Views.Author;
using BookManagement.Views.Reader;
using BookManagement.Views.Admin;
using BookManagement.Views.Common;

namespace BookManagement.Controls
{
    public partial class Sidebar : UserControl
    {
        public Sidebar()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string role = "";

            // 1. Try to find the role name of the logged-in user from the SQL database
            if (BookManagement.Services.Utils.UserSession.CurrentUser != null)
            {
                var account = BookManagement.Services.Utils.UserSession.CurrentUser;
                using (var db = new BookManagement.WPF.Entities.ProjectPrnContext())
                {
                    var roleObj = db.Roles.FirstOrDefault(r => r.RoleId == account.RoleId);
                    if (roleObj != null)
                    {
                        role = roleObj.RoleName;
                    }
                }
            }

            // 2. Fallback or identify based on the DataContext type (Dashboard ViewModel)
            if (string.IsNullOrEmpty(role) && DataContext != null)
            {
                string vmName = DataContext.GetType().Name;
                if (vmName.Contains("Author")) role = "Author";
                else if (vmName.Contains("Reader")) role = "Reader";
                else if (vmName.Contains("Admin")) role = "Admin";
            }

            // 3. Show / Hide role-specific menu sections
            AuthorMenu.Visibility = (role == "Author") ? Visibility.Visible : Visibility.Collapsed;
            ReaderMenu.Visibility = (role == "Reader") ? Visibility.Visible : Visibility.Collapsed;
            AdminMenu.Visibility = (role == "Admin") ? Visibility.Visible : Visibility.Collapsed;
        }

        private DependencyObject GetParentPage()
        {
            DependencyObject parent = this;
            while (parent != null && parent is not Page)
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        private AuthorDashboardViewModel GetAuthorVm()
        {
            return (GetParentPage() as Page)?.DataContext as AuthorDashboardViewModel;
        }

        private ReaderDashboardViewModel GetReaderVm()
        {
            return (GetParentPage() as Page)?.DataContext as ReaderDashboardViewModel;
        }

        private AdminDashboardViewModel GetAdminVm()
        {
            return (GetParentPage() as Page)?.DataContext as AdminDashboardViewModel;
        }

        // ==========================================
        // AUTHOR NAVIGATION CLICK EVENTS
        // ==========================================
        private void AuthorHome_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAuthorVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AuthorHomeViewModel(vm, App.Current.Services.GetRequiredService<IBookService>());
            }
        }

        private void AuthorBooks_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAuthorVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AuthorBooksViewModel(vm, App.Current.Services.GetRequiredService<IBookService>());
            }
        }

        private void AuthorCreateBook_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAuthorVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AuthorCreateBookViewModel(vm, App.Current.Services.GetRequiredService<IBookService>());
            }
        }

        private void AuthorReviewHistory_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAuthorVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AuthorReviewHistoryViewModel(vm, App.Current.Services.GetRequiredService<IReviewService>());
            }
        }

        private void AuthorProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAuthorVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AuthorProfileViewModel(vm, App.Current.Services.GetRequiredService<IAuthorService>());
            }
        }

        // ==========================================
        // READER NAVIGATION CLICK EVENTS
        // ==========================================
        private void ReaderHome_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetReaderVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new ReaderHomeViewModel(vm, 
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IPurchaseService>());
            }
        }

        private void ReaderBooks_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetReaderVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new ReaderBooksViewModel(vm,
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IPurchaseService>());
            }
        }

        private void ReaderFavorite_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetReaderVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new ReaderFavoriteViewModel(vm,
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IPurchaseService>());
            }
        }

        private void ReaderLibrary_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetReaderVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new ReaderLibraryViewModel(vm,
                    App.Current.Services.GetRequiredService<IPurchaseService>(),
                    App.Current.Services.GetRequiredService<IBookService>());
            }
        }

        private void ReaderProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetReaderVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new ReaderProfileViewModel(vm, App.Current.Services.GetRequiredService<IReaderService>());
            }
        }

        // ==========================================
        // ADMIN NAVIGATION CLICK EVENTS
        // ==========================================
        private void AdminHome_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminHomeViewModel(vm, 
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IReaderService>(),
                    App.Current.Services.GetRequiredService<IAuthorService>(),
                    App.Current.Services.GetRequiredService<IPurchaseService>(),
                    App.Current.Services.GetRequiredService<IReviewService>());
            }
        }

        private void AdminAllBooks_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminAllBooksViewModel(vm,
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IReviewService>(),
                    App.Current.Services.GetRequiredService<IAuthorService>());
            }
        }

        private void AdminPendingBooks_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminPendingBooksViewModel(vm,
                    App.Current.Services.GetRequiredService<IBookService>(),
                    App.Current.Services.GetRequiredService<IAuthorService>(),
                    App.Current.Services.GetRequiredService<IReviewService>());
            }
        }

        private void AdminUsers_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminUsersViewModel(vm, 
                    App.Current.Services.GetRequiredService<IReaderService>(),
                    App.Current.Services.GetRequiredService<IAuthorService>(),
                    App.Current.Services.GetRequiredService<IPurchaseService>(),
                    App.Current.Services.GetRequiredService<IBookService>());
            }
        }

        private void AdminReports_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminReportsViewModel(vm,
                    App.Current.Services.GetRequiredService<IPurchaseService>(),
                    App.Current.Services.GetRequiredService<IReaderService>(),
                    App.Current.Services.GetRequiredService<IAuthorService>());
            }
        }

        private void AdminProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetAdminVm();
            if (vm != null)
            {
                vm.CurrentPageViewModel = new AdminProfileViewModel(vm, App.Current.Services.GetRequiredService<IUserService>());
            }
        }

        // ==========================================
        // COMMON LOGOUT EVENT
        // ==========================================
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Reset main frame to login screen
            NavigationService.Instance.NavigateMain(new Login());
        }
    }
}
