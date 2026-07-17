using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminUsersViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;
        private readonly IPurchaseService _purchaseService;
        private readonly IBookService _bookService;

        private string _searchText = string.Empty;
        private string _selectedRole = "All";
        private string _selectedStatus = "All";

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    UsersView.Refresh();
                }
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    UsersView.Refresh();
                }
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    UsersView.Refresh();
                }
            }
        }

        public ObservableCollection<UserItemViewModel> Users { get; } = new ObservableCollection<UserItemViewModel>();
        public ICollectionView UsersView { get; }

        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string> { "All", "Reader", "Author" };
        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string> { "All", "Active", "Inactive" };

        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public AdminUsersViewModel(
            DashboardViewModelBase dashboard,
            IReaderService readerService,
            IAuthorService authorService,
            IPurchaseService purchaseService,
            IBookService bookService)
        {
            _dashboard = dashboard;
            _readerService = readerService;
            _authorService = authorService;
            _purchaseService = purchaseService;
            _bookService = bookService;

            RefreshCommand = new RelayCommand(LoadUsers);
            ViewDetailCommand = new RelayCommand<UserItemViewModel>(OnViewDetail);
            EditUserCommand = new RelayCommand<UserItemViewModel>(OnEditUser);
            DeleteUserCommand = new RelayCommand<UserItemViewModel>(OnDeleteUser);

            UsersView = CollectionViewSource.GetDefaultView(Users);
            UsersView.Filter = FilterUsers;

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            // Load Readers
            foreach (var r in _readerService.GetAllReaders())
            {
                Users.Add(new UserItemViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Email = r.Email,
                    Role = "Reader",
                    Status = r.Status,
                    JoinedDate = r.JoinedDate,
                    Phone = r.Phone,
                    Address = r.Address
                });
            }

            // Load Authors
            foreach (var a in _authorService.GetAllAuthors())
            {
                Users.Add(new UserItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Email = a.Email,
                    Role = "Author",
                    Status = a.Status,
                    JoinedDate = a.JoinedDate,
                    Phone = a.Phone,
                    Address = a.Address,
                    Bio = a.Bio
                });
            }
        }

        private bool FilterUsers(object item)
        {
            if (!(item is UserItemViewModel user)) return false;

            // Search Filter
            if (!string.IsNullOrEmpty(SearchText))
            {
                bool matchesSearch = user.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                     user.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                if (!matchesSearch) return false;
            }

            // Role Filter
            if (SelectedRole != "All")
            {
                if (user.Role != SelectedRole) return false;
            }

            // Status Filter
            if (SelectedStatus != "All")
            {
                if (user.Status != SelectedStatus) return false;
            }

            return true;
        }

        private void OnViewDetail(UserItemViewModel user)
        {
            if (user == null) return;

            if (user.Role == "Reader")
            {
                var reader = _readerService.GetReaderById(user.Id);
                if (reader != null)
                {
                    _dashboard.PageTitle = "Reader Detail";
                    _dashboard.CurrentPageViewModel = new AdminReaderDetailViewModel(_dashboard, _readerService, _purchaseService, _bookService, _authorService, reader);
                }
            }
            else
            {
                _dashboard.ShowToast($"Chi tiết tác giả: {user.Name}", "Info");
            }
        }

        private void OnEditUser(UserItemViewModel user)
        {
            if (user == null) return;

            var editVm = new EditUserViewModel(user);
            var window = new EditUserWindow
            {
                DataContext = editVm,
                Owner = Application.Current?.MainWindow
            };

            if (window.ShowDialog() != true) return;

            bool ok;
            if (user.Role == "Reader")
            {
                ok = _readerService.UpdateReader(new ReaderModel
                {
                    Id = user.Id,
                    Name = editVm.Name,
                    Email = editVm.Email,
                    Phone = editVm.Phone,
                    Address = editVm.Address,
                    Status = editVm.SelectedStatus
                });
            }
            else
            {
                ok = _authorService.UpdateAuthor(new AuthorModel
                {
                    Id = user.Id,
                    Name = editVm.Name,
                    Email = editVm.Email,
                    Phone = editVm.Phone,
                    Address = editVm.Address,
                    Status = editVm.SelectedStatus
                });
            }

            if (ok)
            {
                LoadUsers();
                UsersView.Refresh();
                _dashboard.ShowToast($"Đã cập nhật người dùng: {editVm.Name}", "Success");
            }
            else
            {
                _dashboard.ShowToast("Cập nhật thất bại (email có thể đã tồn tại).", "Warning");
            }
        }

        private void OnDeleteUser(UserItemViewModel user)
        {
            if (user == null) return;

            var confirm = MessageBox.Show(
                $"Vô hiệu hóa người dùng \"{user.Name}\"?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            if (user.Role == "Reader")
            {
                _readerService.SetReaderActive(user.Id, false);
            }
            else
            {
                _authorService.SetAuthorActive(user.Id, false);
            }

            LoadUsers();
            UsersView.Refresh();
            _dashboard.ShowToast($"Đã vô hiệu hóa người dùng: {user.Name}", "Success");
        }
    }
}
