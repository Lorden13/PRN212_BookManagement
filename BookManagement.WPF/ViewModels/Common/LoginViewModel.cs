using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services;

namespace BookManagement.ViewModels.Common
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BookManagement.Helpers;
    using BookManagement.Services.Repository;

    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private bool _isRegisterMode;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _registerFullName = string.Empty;
        private string _registerEmail = string.Empty;
        private string _registerPassword = string.Empty;
        private string _registerRole = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoginPasswordVisible;
        private bool _isRegisterPasswordVisible;

        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set
            {
                if (SetProperty(ref _isRegisterMode, value))
                {
                    // Clear fields when switching modes
                    Email = string.Empty;
                    Password = string.Empty;
                    RegisterFullName = string.Empty;
                    RegisterEmail = string.Empty;
                    RegisterPassword = string.Empty;
                    RegisterRole = string.Empty;
                    ErrorMessage = string.Empty;
                    IsLoginPasswordVisible = false;
                    IsRegisterPasswordVisible = false;
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterFullName
        {
            get => _registerFullName;
            set => SetProperty(ref _registerFullName, value);
        }

        public string RegisterEmail
        {
            get => _registerEmail;
            set => SetProperty(ref _registerEmail, value);
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set => SetProperty(ref _registerPassword, value);
        }

        public string RegisterRole
        {
            get => _registerRole;
            set => SetProperty(ref _registerRole, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoginPasswordVisible
        {
            get => _isLoginPasswordVisible;
            set => SetProperty(ref _isLoginPasswordVisible, value);
        }

        public bool IsRegisterPasswordVisible
        {
            get => _isRegisterPasswordVisible;
            set => SetProperty(ref _isRegisterPasswordVisible, value);
        }

        public ObservableCollection<string> Roles { get; } = new() { "Reader", "Author" };

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }
        public ICommand ToggleLoginPasswordVisibilityCommand { get; }
        public ICommand ToggleRegisterPasswordVisibilityCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister, CanRegister);
            SwitchToRegisterCommand = new RelayCommand(() => IsRegisterMode = true);
            SwitchToLoginCommand = new RelayCommand(() => IsRegisterMode = false);
            ToggleLoginPasswordVisibilityCommand = new RelayCommand(() => IsLoginPasswordVisible = !IsLoginPasswordVisible);
            ToggleRegisterPasswordVisibilityCommand = new RelayCommand(() => IsRegisterPasswordVisible = !IsRegisterPasswordVisible);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            ErrorMessage = string.Empty;

            // TODO: Replace with BookManagement.API authentication
            if (Email == "reader@demo.com" && Password == "123456")
            {
                NavigationService.Instance.NavigateMain(new ReaderDashboard());
            }
            else if (Email == "author@demo.com" && Password == "123456")
            {
                NavigationService.Instance.NavigateMain(new AuthorDashboard());
            }
            else if (Email == "admin@demo.com" && Password == "123456")
            {
                NavigationService.Instance.NavigateMain(new AdminDashboard());
            }
            else
            {
                ErrorMessage = "Email hoặc mật khẩu không chính xác. Vui lòng thử lại.";
            }
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(RegisterFullName) &&
                   !string.IsNullOrWhiteSpace(RegisterEmail) &&
                   !string.IsNullOrWhiteSpace(RegisterPassword) &&
                   !string.IsNullOrWhiteSpace(RegisterRole);
        }

        private void ExecuteRegister()
        {
            // Placeholder command
        }
    }
}
