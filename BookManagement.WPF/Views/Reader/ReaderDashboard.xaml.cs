using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BookManagement.ViewModels.Reader;
using BookManagement.Views.Reader;

namespace BookManagement.Views.Reader
{
    public partial class ReaderDashboard : Page
    {
        public ReaderDashboard()
        {
            InitializeComponent();
            var vm = App.Current.Services.GetRequiredService<BookManagement.ViewModels.Reader.ReaderDashboardViewModel>();
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
            vm.CurrentPageViewModel = new BookManagement.ViewModels.Reader.ReaderHomeViewModel(vm, 
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IBookService>(),
                App.Current.Services.GetRequiredService<BookManagement.Services.Repository.IPurchaseService>());
        }

        private void NavigateToViewModel(object viewModel)
        {
            if (viewModel == null) return;

            UserControl view = null;
            if (viewModel is ReaderHomeViewModel vmHome)
            {
                view = new ReaderHomeView { DataContext = vmHome };
            }
            else if (viewModel is ReaderBooksViewModel vmBooks)
            {
                view = new ReaderBooksView { DataContext = vmBooks };
            }
            else if (viewModel is ReaderBookDetailViewModel vmDetail)
            {
                view = new ReaderBookDetailView { DataContext = vmDetail };
            }
            else if (viewModel is ReaderFavoriteViewModel vmFav)
            {
                view = new ReaderFavoriteView { DataContext = vmFav };
            }
            else if (viewModel is ReaderLibraryViewModel vmLib)
            {
                view = new ReaderLibraryView { DataContext = vmLib };
            }
            else if (viewModel is ReaderProfileViewModel vmProfile)
            {
                view = new ReaderProfileView { DataContext = vmProfile };
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
