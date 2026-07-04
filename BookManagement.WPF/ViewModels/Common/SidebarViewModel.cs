using System.Collections.ObjectModel;
using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services;

namespace BookManagement.ViewModels.Common
{
    public class SidebarViewModel : BaseViewModel
    {
        private UserProfileModel _currentUser;
        private MenuItemViewModel? _selectedItem;

        public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new ObservableCollection<MenuItemViewModel>();

        public UserProfileModel CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public void NotifyCurrentUserChanged()
        {
            OnPropertyChanged(nameof(CurrentUser));
        }

        public MenuItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    // Deselect others
                    foreach (var item in MenuItems)
                    {
                        item.IsSelected = (item == value);
                    }

                    // Execute command associated with item (e.g. Navigating nested page)
                    if (value.Command != null && value.Command.CanExecute(null))
                    {
                        value.Command.Execute(null);
                    }
                }
            }
        }

        public ICommand LogoutCommand { get; }

        public SidebarViewModel(UserProfileModel currentUser)
        {
            _currentUser = currentUser;
            LogoutCommand = new RelayCommand(OnLogout);
        }

        private void OnLogout()
        {
            // Reset main frame to login screen
            NavigationService.Instance.NavigateMain(new Login());
        }
    }
}
