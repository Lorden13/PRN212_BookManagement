using System.Windows;
using BookManagement.WPF.Commands;

namespace BookManagement.WPF.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _email = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public ForgotPasswordViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SendCommand = new RelayCommand(_ => Send());
            NavigateToLoginCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("Login"));
        }

        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); HasError = false; }
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

        public RelayCommand SendCommand { get; }
        public RelayCommand NavigateToLoginCommand { get; }

        private void Send()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                ErrorMessage = "Please enter a valid email address.";
                HasError = true;
                return;
            }

            MessageBox.Show("A password reset link has been sent to your email.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _mainViewModel.NavigateTo("Login");
        }
    }
}
