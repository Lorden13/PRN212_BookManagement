using BookManagement.WPF.Commands;

namespace BookManagement.WPF.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmNewPassword = string.Empty;
        private string _passwordMessage = string.Empty;
        private bool _hasPasswordMessage;
        private bool _isPasswordMessageSuccess;

        public ChangePasswordViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword());
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set => SetProperty(ref _confirmNewPassword, value);
        }

        public string PasswordMessage
        {
            get => _passwordMessage;
            set => SetProperty(ref _passwordMessage, value);
        }

        public bool HasPasswordMessage
        {
            get => _hasPasswordMessage;
            set => SetProperty(ref _hasPasswordMessage, value);
        }

        public bool IsPasswordMessageSuccess
        {
            get => _isPasswordMessageSuccess;
            set => SetProperty(ref _isPasswordMessageSuccess, value);
        }

        public RelayCommand ChangePasswordCommand { get; }

        private void ChangePassword()
        {
            var user = _mainViewModel.CurrentUser;
            if (user == null) return;

            if (string.IsNullOrEmpty(CurrentPassword))
            {
                ShowPasswordMessage("Please enter your current password.", false);
                return;
            }

            if (user.Password != CurrentPassword)
            {
                ShowPasswordMessage("Current password is incorrect.", false);
                return;
            }

            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 6)
            {
                ShowPasswordMessage("New password must be at least 6 characters.", false);
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ShowPasswordMessage("Confirm password does not match.", false);
                return;
            }

            user.Password = NewPassword;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            ShowPasswordMessage("Password changed successfully!", true);
        }

        private void ShowPasswordMessage(string msg, bool isSuccess)
        {
            PasswordMessage = msg;
            HasPasswordMessage = true;
            IsPasswordMessageSuccess = isSuccess;
        }
    }
}
