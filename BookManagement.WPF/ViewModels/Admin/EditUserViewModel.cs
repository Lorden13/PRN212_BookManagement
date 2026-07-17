using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.ViewModels.Common;

namespace BookManagement.ViewModels.Admin
{
    public class EditUserViewModel : BaseViewModel
    {
        private string _name;
        private string _email;
        private string _phone;
        private string _address;
        private string _selectedStatus;

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

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        // Read-only display info
        public string Role { get; }
        public string JoinedDate { get; }

        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string> { "Active", "Inactive" };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? CloseRequested;

        public EditUserViewModel(UserItemViewModel user)
        {
            Name = user.Name;
            Email = user.Email;
            Phone = user.Phone;
            Address = user.Address;
            SelectedStatus = user.Status;
            Role = user.Role;
            JoinedDate = user.JoinedDate;

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Tên không được để trống!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Email không được để trống!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không đúng định dạng!", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CloseRequested?.Invoke(true);
        }

        private void OnCancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
