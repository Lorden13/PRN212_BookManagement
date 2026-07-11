using BookManagement.Services.Utils;

namespace BookManagement.ViewModels.Author
{
    public class AuthorProfileViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IAuthorService _authorService;
        private readonly AuthorModel? _authorModel;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _bio = string.Empty;
        private string _joinedDate = string.Empty;

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

        public string Bio
        {
            get => _bio;
            set => SetProperty(ref _bio, value);
        }

        public string JoinedDate
        {
            get => _joinedDate;
            set => SetProperty(ref _joinedDate, value);
        }

        public ICommand SaveCommand { get; }

        public AuthorProfileViewModel(DashboardViewModelBase dashboard, IAuthorService authorService)
        {
            _dashboard = dashboard;
            _authorService = authorService;

            SaveCommand = new RelayCommand(OnSave);

            // Load author profile dynamically from current logged-in user session
            if (UserSession.CurrentUser != null)
            {
                _authorModel = _authorService.GetAuthorByAccountId(UserSession.CurrentUser.AccountId);
            }
            else
            {
                _authorModel = _authorService.GetAuthorById(1);
            }

            if (_authorModel != null)
            {
                Name = _authorModel.Name;
                Email = _authorModel.Email;
                Bio = _authorModel.Bio;
                JoinedDate = _authorModel.JoinedDate;
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                _dashboard.ShowToast("Tên và Email không được để trống!", "Warning");
                return;
            }

            if (_authorModel != null)
            {
                _authorModel.Name = Name;
                _authorModel.Email = Email;
                _authorModel.Bio = Bio;

                _authorService.UpdateProfile(_authorModel);

                // Update Dashboard header & sidebar info
                _dashboard.Sidebar.CurrentUser.Name = Name;
                _dashboard.Sidebar.CurrentUser.Email = Email;
                _dashboard.Sidebar.NotifyCurrentUserChanged();

                _dashboard.ShowToast("Đã cập nhật thông tin tác giả!", "Success");
            }
        }
    }
}
