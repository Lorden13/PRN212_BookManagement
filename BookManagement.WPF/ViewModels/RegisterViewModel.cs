using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isAuthorRole = true;
        private bool _isReaderRole;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        private string _address = string.Empty;
        private string _phoneNumber = string.Empty;

        public RegisterViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            RegisterCommand = new RelayCommand(_ => Register());
            NavigateToLoginCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("Login"));
        }

        public string FullName
        {
            get => _fullName;
            set { SetProperty(ref _fullName, value); HasError = false; }
        }

        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); HasError = false; }
        }

        public string Address
        {
            get => _address;
            set { SetProperty(ref _address, value); HasError = false; }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { SetProperty(ref _phoneNumber, value); HasError = false; }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsAuthorRole
        {
            get => _isAuthorRole;
            set => SetProperty(ref _isAuthorRole, value);
        }

        public bool IsReaderRole
        {
            get => _isReaderRole;
            set => SetProperty(ref _isReaderRole, value);
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

        public RelayCommand RegisterCommand { get; }
        public RelayCommand NavigateToLoginCommand { get; }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Please enter your full name.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                ErrorMessage = "Please enter a valid email address.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                ErrorMessage = "Please enter your address.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ErrorMessage = "Please enter your phone number.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                HasError = true;
                return;
            }

            // Create new user in mock data
            var newUser = new User
            {
                Id = Helpers.MockDataService.Users.Count + 1,
                FullName = FullName,
                Email = Email,
                Password = Password,
                Role = IsAuthorRole ? UserRole.Author : UserRole.Reader,
                Status = UserStatus.Active,
                Address = Address,
                PhoneNumber = PhoneNumber
            };
            Helpers.MockDataService.Users.Add(newUser);

            MessageBox.Show("Registration successful! Please login with your credentials.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _mainViewModel.NavigateTo("Login");
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetConfirmPassword(string password)
        {
            ConfirmPassword = password;
        }
    }
}
