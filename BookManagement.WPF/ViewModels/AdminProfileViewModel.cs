using BookManagement.WPF.Commands;

namespace BookManagement.WPF.ViewModels
{
    public class AdminProfileViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _address = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _profileMessage = string.Empty;
        private bool _hasProfileMessage;
        private bool _isProfileMessageSuccess;

        public AdminProfileViewModel(MainViewModel mainViewModel)
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
            ChangePasswordViewModel = new ChangePasswordViewModel(mainViewModel);
        }

        public ChangePasswordViewModel ChangePasswordViewModel { get; }

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

        public RelayCommand SaveProfileCommand { get; }

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

        private void ShowProfileMessage(string msg, bool isSuccess)
        {
            ProfileMessage = msg;
            HasProfileMessage = true;
            IsProfileMessageSuccess = isSuccess;
        }
    }
}
