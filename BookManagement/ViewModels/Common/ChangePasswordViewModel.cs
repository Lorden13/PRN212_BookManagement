using System;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Common
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IUserService _userService;

        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand UpdatePasswordCommand { get; }

        public ChangePasswordViewModel(DashboardViewModelBase dashboard, IUserService userService)
        {
            _dashboard = dashboard;
            _userService = userService;

            UpdatePasswordCommand = new RelayCommand(OnUpdatePassword);
        }

        private void OnUpdatePassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                _dashboard.ShowToast("Vui lòng điền đầy đủ các trường!", "Warning");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                _dashboard.ShowToast("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Warning");
                return;
            }

            if (NewPassword.Length < 6)
            {
                _dashboard.ShowToast("Mật khẩu mới phải dài từ 6 ký tự!", "Warning");
                return;
            }

            // Simulate password update
            _dashboard.ShowToast("Đã đổi mật khẩu thành công!", "Success");
            CurrentPassword = "";
            NewPassword = "";
            ConfirmPassword = "";
        }
    }
}
