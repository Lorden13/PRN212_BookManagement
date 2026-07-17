using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Admin
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();
            var vm = App.Current.Services.GetRequiredService<BookManagement.ViewModels.Admin.AdminDashboardViewModel>();
            DataContext = vm;

            // Register Content Frame for inner page navigation
            BookManagement.Services.Navigation.NavigationService.Instance.RegisterContentFrame(ContentFrame);

            // Listen to property changes in the ViewModel for dynamic inner navigation
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.CurrentPageViewModel))
                {
                    NavigateToViewModel(vm.CurrentPageViewModel);
                }
            };

            // Set default view on load
            vm.CurrentPageViewModel = new BookManagement.ViewModels.Admin.AdminHomeViewModel(vm, 
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IBookService>(),
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IReaderService>(),
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IAuthorService>(),
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IPurchaseService>(),
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IReviewService>());
        }

        private void NavigateToViewModel(object viewModel)
        {
            if (viewModel == null) return;

            UserControl view = null;
            if (viewModel is AdminHomeViewModel vmHome)
            {
                view = new AdminHomeView { DataContext = vmHome };
            }
            else if (viewModel is AdminUsersViewModel vmUsers)
            {
                view = new AdminUsersView { DataContext = vmUsers };
            }
            else if (viewModel is AdminReaderDetailViewModel vmReader)
            {
                view = new AdminReaderDetailView { DataContext = vmReader };
            }
            else if (viewModel is AdminPendingBooksViewModel vmPending)
            {
                view = new AdminPendingBooksView { DataContext = vmPending };
            }
            else if (viewModel is AdminBookReviewViewModel vmReview)
            {
                view = new AdminBookReviewView { DataContext = vmReview };
            }
            else if (viewModel is AdminAllBooksViewModel vmAll)
            {
                view = new AdminAllBooksView { DataContext = vmAll };
            }
            else if (viewModel is AdminPurchasesViewModel vmPurchases)
            {
                view = new AdminPurchasesView { DataContext = vmPurchases };
            }
            else if (viewModel is AdminReviewHistoryViewModel vmHistory)
            {
                view = new AdminReviewHistoryView { DataContext = vmHistory };
            }
            else if (viewModel is AdminReportsViewModel vmReports)
            {
                view = new AdminReportsView { DataContext = vmReports };
            }
            else if (viewModel is AdminProfileViewModel vmProfile)
            {
                view = new AdminProfileView { DataContext = vmProfile };
            }
            else if (viewModel is ChangePasswordViewModel vmPwd)
            {
                view = new ChangePasswordView { DataContext = vmPwd };
            }

            if (view != null)
            {
                BookManagement.Services.Navigation.NavigationService.Instance.NavigateContent(view);
            }
        }
    }
}
