using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;

namespace BookManagement.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            LoginCommand = new RelayCommand(_ => Login());
            NavigateToRegisterCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("Register"));
            NavigateToForgotPasswordCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("ForgotPassword"));
        }

        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); HasError = false; }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand NavigateToRegisterCommand { get; }
        public RelayCommand NavigateToForgotPasswordCommand { get; }

        private void Login()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                HasError = true;
                return;
            }

            if (!Email.Contains("@"))
            {
                ErrorMessage = "Please enter a valid email address.";
                HasError = true;
                return;
            }

            // Authenticate
            var user = MockDataService.Authenticate(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid email or password. Please try again.";
                HasError = true;
                return;
            }

            // Success — navigate to dashboard
            _mainViewModel.Login(user);
        }

        // Called from code-behind to set password (PasswordBox can't bind)
        public void SetPassword(string password)
        {
            Password = password;
        }
    }
}
