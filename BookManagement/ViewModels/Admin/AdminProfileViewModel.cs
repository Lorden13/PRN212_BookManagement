using System;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminProfileViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IUserService _userService;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _address = string.Empty;
        private string _role = "Admin";
        private string _joinedDate = string.Empty;
        private string _avatarPath = string.Empty;

        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string JoinedDate
        {
            get => _joinedDate;
            set => SetProperty(ref _joinedDate, value);
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set => SetProperty(ref _avatarPath, value);
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand SelectAvatarCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminProfileViewModel(DashboardViewModelBase dashboard, IUserService userService)
        {
            _dashboard = dashboard;
            _userService = userService;

            SaveCommand = new RelayCommand(OnSave);
            SelectAvatarCommand = new RelayCommand(OnSelectAvatar);
            ChangePasswordCommand = new RelayCommand(OnChangePassword);
            LogoutCommand = new RelayCommand(OnLogout);

            var adminUser = _dashboard.Sidebar.CurrentUser;
            if (adminUser != null)
            {
                Name = adminUser.Name;
                Email = adminUser.Email;
                Role = adminUser.Role;
                JoinedDate = adminUser.JoinedDate;
                AvatarPath = adminUser.AvatarPath ?? "IconShield";
                Phone = "0111 222 333";
                Address = "Hệ thống máy chủ chính";
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                _dashboard.ShowToast("Tên và Email không được để trống!", "Warning");
                return;
            }

            var adminUser = _dashboard.Sidebar.CurrentUser;
            if (adminUser != null)
            {
                adminUser.Name = Name;
                adminUser.Email = Email;
                adminUser.AvatarPath = AvatarPath;

                _dashboard.Sidebar.NotifyCurrentUserChanged();
                _dashboard.ShowToast("Đã cập nhật thông tin cá nhân của Admin!", "Success");
            }
        }

        private void OnSelectAvatar()
        {
            if (AvatarPath == "IconShield" || string.IsNullOrEmpty(AvatarPath))
            {
                AvatarPath = "IconUser";
            }
            else if (AvatarPath == "IconUser")
            {
                AvatarPath = "IconHome";
            }
            else
            {
                AvatarPath = "IconShield";
            }
            _dashboard.ShowToast("Đã thay đổi ảnh đại diện Admin!", "Info");
        }

        private void OnChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                _dashboard.ShowToast("Vui lòng điền đầy đủ thông tin mật khẩu!", "Warning");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                _dashboard.ShowToast("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Warning");
                return;
            }

            _dashboard.ShowToast("Đổi mật khẩu thành công!", "Success");
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        private void OnLogout()
        {
            _dashboard.Sidebar.LogoutCommand.Execute(null);
        }
    }
}
