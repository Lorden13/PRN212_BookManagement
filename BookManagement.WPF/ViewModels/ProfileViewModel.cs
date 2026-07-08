using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _address = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmNewPassword = string.Empty;
        private string _profileMessage = string.Empty;
        private bool _hasProfileMessage;
        private bool _isProfileMessageSuccess;
        private string _passwordMessage = string.Empty;
        private bool _hasPasswordMessage;
        private bool _isPasswordMessageSuccess;

        public ProfileViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            var user = _mainViewModel.CurrentUser;
            if (user != null)
            {
                FullName = user.FullName;
                Email = user.Email;
                Address = user.Address;
                PhoneNumber = user.PhoneNumber;
            }

            SaveProfileCommand = new RelayCommand(_ => SaveProfile());
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword());
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
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

        public string ProfileMessage
        {
            get => _profileMessage;
            set => SetProperty(ref _profileMessage, value);
        }

        public bool HasProfileMessage
        {
            get => _hasProfileMessage;
            set => SetProperty(ref _hasProfileMessage, value);
        }

        public bool IsProfileMessageSuccess
        {
            get => _isProfileMessageSuccess;
            set => SetProperty(ref _isProfileMessageSuccess, value);
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

        public RelayCommand SaveProfileCommand { get; }
        public RelayCommand ChangePasswordCommand { get; }

        private void SaveProfile()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ShowProfileMessage("Please enter your full name.", false);
                return;
            }

            var user = _mainViewModel.CurrentUser;
            if (user != null)
            {
                user.FullName = FullName;
                user.Address = Address;
                user.PhoneNumber = PhoneNumber;
                _mainViewModel.CurrentUser = user; // Triggers UI update for FullName in main shell
                ShowProfileMessage("Profile updated successfully!", true);
            }
        }

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

        private void ShowProfileMessage(string msg, bool isSuccess)
        {
            ProfileMessage = msg;
            HasProfileMessage = true;
            IsProfileMessageSuccess = isSuccess;
        }

        private void ShowPasswordMessage(string msg, bool isSuccess)
        {
            PasswordMessage = msg;
            HasPasswordMessage = true;
            IsPasswordMessageSuccess = isSuccess;
        }
    }
}
