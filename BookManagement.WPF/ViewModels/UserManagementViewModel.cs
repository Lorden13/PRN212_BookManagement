using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class UserManagementViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _searchText = string.Empty;

        public UserManagementViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SearchCommand = new RelayCommand(_ => ApplySearch());
            ViewCommand = new RelayCommand(param =>
            {
                if (param is User user)
                    MessageBox.Show($"User Details:\n\nName: {user.FullName}\nEmail: {user.Email}\nRole: {user.Role}\nStatus: {user.Status}", "User Details", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            EditCommand = new RelayCommand(param =>
            {
                if (param is User user)
                    MessageBox.Show($"Edit user \"{user.FullName}\" — coming soon!", "Edit User", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            DeleteCommand = new RelayCommand(param =>
            {
                if (param is User user)
                    MessageBox.Show($"Are you sure you want to delete \"{user.FullName}\"?\n\n(Demo only — no data will be changed)", "Confirm Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            });

            LoadUsers();
        }

        public ObservableCollection<User> Users { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplySearch();
            }
        }

        public bool HasUsers => Users.Count > 0;

        public RelayCommand SearchCommand { get; }
        public RelayCommand ViewCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        private void LoadUsers()
        {
            Users.Clear();
            foreach (var user in MockDataService.Users)
                Users.Add(user);
            OnPropertyChanged(nameof(HasUsers));
        }

        private void ApplySearch()
        {
            Users.Clear();
            var filtered = MockDataService.Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.Trim();
                filtered = filtered.Where(u =>
                    u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var user in filtered)
                Users.Add(user);

            OnPropertyChanged(nameof(HasUsers));
        }
    }
}
