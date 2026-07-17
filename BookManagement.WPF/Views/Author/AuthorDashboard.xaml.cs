using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Author
{
    public partial class AuthorDashboard : Page
    {
        public AuthorDashboard()
        {
            Console.WriteLine("[AuthorDashboard] Constructor started");
            try
            {
                InitializeComponent();
                var vm = App.Current.Services.GetRequiredService<BookManagement.ViewModels.Author.AuthorDashboardViewModel>();
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
                vm.CurrentPageViewModel = new BookManagement.ViewModels.Author.AuthorHomeViewModel(vm, App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IBookService>());
                Console.WriteLine("[AuthorDashboard] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthorDashboard] Constructor threw exception: {ex}");
                throw;
            }
        }

        private void NavigateToViewModel(object viewModel)
        {
            if (viewModel == null) return;

            UserControl view = null;
            if (viewModel is AuthorHomeViewModel vmHome)
            {
                view = new AuthorHomeView { DataContext = vmHome };
            }
            else if (viewModel is AuthorBooksViewModel vmBooks)
            {
                view = new AuthorBooksView { DataContext = vmBooks };
            }
            else if (viewModel is AuthorCreateBookViewModel vmCreate)
            {
                view = new AuthorCreateBookView { DataContext = vmCreate };
            }
            else if (viewModel is AuthorBookDetailViewModel vmDetail)
            {
                view = new AuthorBookDetailView { DataContext = vmDetail };
            }
            else if (viewModel is AuthorReviewHistoryViewModel vmReview)
            {
                view = new AuthorReviewHistoryView { DataContext = vmReview };
            }
            else if (viewModel is AuthorProfileViewModel vmProfile)
            {
                view = new AuthorProfileView { DataContext = vmProfile };
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
