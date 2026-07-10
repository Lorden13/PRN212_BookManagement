using BookManagement.Helpers;
using BookManagement.Services;
using System.Windows.Input;

namespace BookManagement.ViewModels.Common
{
    using BookManagement.Helpers;
    using BookManagement.Services.Repository;
    using BookManagement.WPF.Entities;
    using BookManagement.WPF.Services.AuthorSetvice;
    using BookManagement.WPF.Services.ReaderService;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BookManagement.Services.Utils;

    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private bool _isRegisterMode;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _registerFullName = string.Empty;
        private string _registerEmail = string.Empty;
        private string _registerPhone = string.Empty;
        private string _registerAddress = string.Empty;
        private string _registerPassword = string.Empty;
        private string _registerConfirmPassword = string.Empty;
        private string _registerRole = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoginPasswordVisible;
        private bool _isRegisterPasswordVisible;
        private bool _isRegisterConfirmPasswordVisible;

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
                    RegisterPhone = string.Empty;
                    RegisterAddress = string.Empty;
                    RegisterPassword = string.Empty;
                    RegisterConfirmPassword = string.Empty;
                    RegisterRole = string.Empty;
                    ErrorMessage = string.Empty;
                    IsLoginPasswordVisible = false;
                    IsRegisterPasswordVisible = false;
                    IsRegisterConfirmPasswordVisible = false;
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
            set
            {
                if (SetProperty(ref _registerFullName, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterEmail
        {
            get => _registerEmail;
            set
            {
                if (SetProperty(ref _registerEmail, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterPhone
        {
            get => _registerPhone;
            set
            {
                if (SetProperty(ref _registerPhone, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterAddress
        {
            get => _registerAddress;
            set
            {
                if (SetProperty(ref _registerAddress, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set
            {
                if (SetProperty(ref _registerPassword, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterConfirmPassword
        {
            get => _registerConfirmPassword;
            set
            {
                if (SetProperty(ref _registerConfirmPassword, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string RegisterRole
        {
            get => _registerRole;
            set
            {
                if (SetProperty(ref _registerRole, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
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

        public bool IsRegisterConfirmPasswordVisible
        {
            get => _isRegisterConfirmPasswordVisible;
            set => SetProperty(ref _isRegisterConfirmPasswordVisible, value);
        }

        public ObservableCollection<string> Roles { get; } = new() { "Reader", "Author", "Admin" };

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }
        public ICommand ToggleLoginPasswordVisibilityCommand { get; }
        public ICommand ToggleRegisterPasswordVisibilityCommand { get; }
        public ICommand ToggleRegisterConfirmPasswordVisibilityCommand { get; }
        public ICommand NavigateToForgotPasswordCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister, CanRegister);
            SwitchToRegisterCommand = new RelayCommand(() => IsRegisterMode = true);
            SwitchToLoginCommand = new RelayCommand(() => IsRegisterMode = false);
            ToggleLoginPasswordVisibilityCommand = new RelayCommand(() => IsLoginPasswordVisible = !IsLoginPasswordVisible);
            ToggleRegisterPasswordVisibilityCommand = new RelayCommand(() => IsRegisterPasswordVisible = !IsRegisterPasswordVisible);
            ToggleRegisterConfirmPasswordVisibilityCommand = new RelayCommand(() => IsRegisterConfirmPasswordVisible = !IsRegisterConfirmPasswordVisible);
            NavigateToForgotPasswordCommand = new RelayCommand(ExecuteNavigateToForgotPassword);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            ErrorMessage = string.Empty;

            //// TODO: Replace with BookManagement.API authentication
            //if (Email == "reader@demo.com" && Password == "123456")
            //{
            //    NavigationService.Instance.NavigateMain(new ReaderDashboard());
            //}
            //else if (Email == "author@demo.com" && Password == "123456")
            //{
            //    NavigationService.Instance.NavigateMain(new AuthorDashboard());
            //}
            //else if (Email == "admin@demo.com" && Password == "123456")
            //{
            //    NavigationService.Instance.NavigateMain(new AdminDashboard());
            //}
            //else
            //{
            //    ErrorMessage = "Email hoặc mật khẩu không chính xác. Vui lòng thử lại.";
            //}
            var account = _userService.Login(Email, Password);

            if (account == null)
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return;
            }

            // Lưu session
            UserSession.CurrentUser = account;

            switch (account.RoleId)
            {
                case "AUTHOR":
                    NavigationService.Instance.NavigateMain(new AuthorDashboard());
                    break;

                case "READER":
                    NavigationService.Instance.NavigateMain(new ReaderDashboard());
                    break;

                case "ADMIN":
                    NavigationService.Instance.NavigateMain(new AdminDashboard());
                    break;
            }


        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(RegisterFullName) &&
                   !string.IsNullOrWhiteSpace(RegisterEmail) &&
                   !string.IsNullOrWhiteSpace(RegisterPhone) &&
                   !string.IsNullOrWhiteSpace(RegisterAddress) &&
                   !string.IsNullOrWhiteSpace(RegisterPassword) &&
                   !string.IsNullOrWhiteSpace(RegisterConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(RegisterRole);
        }

        private void ExecuteRegister()
        {
            ErrorMessage = string.Empty;

            // 1. Email format validation
            if (!IsValidEmail(RegisterEmail))
            {
                ErrorMessage = "Email không hợp lệ. Vui lòng nhập đúng định dạng.";
                return;
            }

            // 2. Phone number digits-only and length validation
            if (!IsNumericOnly(RegisterPhone))
            {
                ErrorMessage = "Số điện thoại chỉ được chứa các chữ số.";
                return;
            }
            if (RegisterPhone.Length > 11)
            {
                ErrorMessage = "Số điện thoại không được vượt quá 11 chữ số.";
                return;
            }

            // 3. Address length validation
            if (RegisterAddress.Length > 255)
            {
                ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.";
                return;
            }

            // 4. Password length validation
            if (RegisterPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.";
                return;
            }

            // 5. Confirm password check
            if (RegisterPassword != RegisterConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return;
            }

            // Simulating successful registration
            System.Windows.MessageBox.Show("Đăng ký tài khoản thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            IsRegisterMode = false;
        }

        private void ExecuteNavigateToForgotPassword()
        {
            NavigationService.Instance.NavigateMain(new ForgotPasswordView());
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsNumericOnly(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, "^[0-9]+$");
        }


    }
}
