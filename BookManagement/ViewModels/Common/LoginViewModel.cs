using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services;

namespace BookManagement.ViewModels.Common
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        public ICommand LoginReaderCommand { get; }
        public ICommand LoginAuthorCommand { get; }
        public ICommand LoginAdminCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginReaderCommand = new RelayCommand(LoginAsReader);
            LoginAuthorCommand = new RelayCommand(LoginAsAuthor);
            LoginAdminCommand = new RelayCommand(LoginAsAdmin);
        }

        private void LoginAsReader()
        {
            NavigationService.Instance.NavigateMain(new ReaderDashboard());
        }

        private void LoginAsAuthor()
        {
            NavigationService.Instance.NavigateMain(new AuthorDashboard());
        }

        private void LoginAsAdmin()
        {
            NavigationService.Instance.NavigateMain(new AdminDashboard());
        }
    }
}
