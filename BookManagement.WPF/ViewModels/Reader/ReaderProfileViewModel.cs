using System;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderProfileViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IReaderService _readerService;
        private readonly ReaderModel? _readerModel;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _joinedDate = string.Empty;
        private string _avatarPath = string.Empty;

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

        public ICommand SaveCommand { get; }
        public ICommand SelectAvatarCommand { get; }

        public ReaderProfileViewModel(DashboardViewModelBase dashboard, IReaderService readerService)
        {
            _dashboard = dashboard;
            _readerService = readerService;

            SaveCommand = new RelayCommand(OnSave);
            SelectAvatarCommand = new RelayCommand(OnSelectAvatar);

            // Load demo reader profile (ID = 1)
            _readerModel = _readerService.GetReaderById(1);
            if (_readerModel != null)
            {
                Name = _readerModel.Name;
                Email = _readerModel.Email;
                Phone = _readerModel.Phone;
                JoinedDate = _readerModel.JoinedDate;
                AvatarPath = _readerModel.AvatarPath ?? "IconUser";
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                _dashboard.ShowToast("Tên và Email không được để trống!", "Warning");
                return;
            }

            if (_readerModel != null)
            {
                _readerModel.Name = Name;
                _readerModel.Email = Email;
                _readerModel.Phone = Phone;
                _readerModel.AvatarPath = AvatarPath;

                _readerService.UpdateProfile(_readerModel);

                // Update Dashboard header & sidebar info
                _dashboard.Sidebar.CurrentUser.Name = Name;
                _dashboard.Sidebar.CurrentUser.Email = Email;
                _dashboard.Sidebar.CurrentUser.AvatarPath = AvatarPath;
                // Force PropertyChanged notifications on nested elements
                _dashboard.Sidebar.NotifyCurrentUserChanged();

                _dashboard.ShowToast("Đã cập nhật thông tin cá nhân!", "Success");
            }
        }

        private void OnSelectAvatar()
        {
            // Cycle through 3 mock avatars for demo purposes
            if (AvatarPath == "IconUser" || string.IsNullOrEmpty(AvatarPath))
            {
                AvatarPath = "IconHome"; // Use geometry keys from Icons.xaml
            }
            else if (AvatarPath == "IconHome")
            {
                AvatarPath = "IconBook";
            }
            else
            {
                AvatarPath = "IconUser";
            }
            _dashboard.ShowToast("Đã thay đổi ảnh đại diện (phác thảo)!", "Info");
        }
    }
}
