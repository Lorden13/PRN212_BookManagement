using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using NavigationService =
BookManagement.Services.Navigation.NavigationService;


namespace BookManagement.Views.Admin
{
    public partial class AdminUsersView : UserControl
    {
        private List<AdminUserItemModel> _allUsers = new List<AdminUserItemModel>();
        private bool _isInitializing = true;

        public AdminUsersView()
        {
            InitializeComponent();

            Loaded += AdminUsersView_Loaded;
        }

        private void AdminUsersView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFilters();
            RefreshData();
            _isInitializing = false;
            ApplyFilters();
        }

        private void InitializeFilters()
        {
            cbRole.ItemsSource = new List<string> { "Tất cả vai trò", "Reader", "Author", "Admin" };
            cbRole.SelectedIndex = 0;

            cbStatus.ItemsSource = new List<string> { "Tất cả trạng thái", "Active", "Inactive" };
            cbStatus.SelectedIndex = 0;
        }

        private void RefreshData()
        {
            try
            {
                using (var db = new ProjectPrnContext())
                {
                    _allUsers = db.Accounts
                        .Include(a => a.Role)
                        .Select(a => new AdminUserItemModel
                        {
                            Id = a.AccountId,
                            Name = a.FullName,
                            Email = a.Email,
                            Phone = a.Phone ?? "N/A",
                            Role = a.Role != null ? a.Role.RoleName : "User",
                            Status = a.IsActive ? "Active" : "Inactive"
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterInput_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string query = txtSearch.Text.Trim().ToLower();
            string roleFilter = cbRole.SelectedItem?.ToString() ?? "Tất cả vai trò";
            string statusFilter = cbStatus.SelectedItem?.ToString() ?? "Tất cả trạng thái";

            var filtered = _allUsers.Where(u =>
            {
                bool matchQuery = string.IsNullOrEmpty(query) || 
                                  u.Name.ToLower().Contains(query) || 
                                  u.Email.ToLower().Contains(query);

                bool matchRole = roleFilter == "Tất cả vai trò" || u.Role.Equals(roleFilter, StringComparison.OrdinalIgnoreCase);
                bool matchStatus = statusFilter == "Tất cả trạng thái" || u.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);

                return matchQuery && matchRole && matchStatus;
            }).ToList();

            dgUsers.ItemsSource = filtered;
            emptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
            ApplyFilters();
        }

        private void BtnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AdminUserItemModel user)
            {
                var nav = NavigationService.GetNavigationService();
                if (nav != null)
                {
                    nav.NavigateContent(new AdminReaderDetailView(user.Id));
                }
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AdminUserItemModel user)
            {
                // Simple toggle active/inactive status
                try
                {
                    using (var db = new ProjectPrnContext())
                    {
                        var acc = db.Accounts.FirstOrDefault(a => a.AccountId == user.Id);
                        if (acc != null)
                        {
                            acc.IsActive = !acc.IsActive;
                            db.SaveChanges();
                            MessageBox.Show($"Đã thay đổi trạng thái hoạt động của {acc.FullName} sang {(acc.IsActive ? "Active" : "Inactive")}.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            RefreshData();
                            ApplyFilters();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cập nhật trạng thái thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AdminUserItemModel user)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản \"{user.Name}\"? Hành động này sẽ xóa vĩnh viễn tài khoản khỏi hệ thống.", "Xác nhận xóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ProjectPrnContext())
                        {
                            var acc = db.Accounts.FirstOrDefault(a => a.AccountId == user.Id);
                            if (acc != null)
                            {
                                db.Accounts.Remove(acc);
                                db.SaveChanges();
                                MessageBox.Show($"Đã xóa tài khoản {acc.FullName}.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                RefreshData();
                                ApplyFilters();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Xóa tài khoản thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    public class AdminUserItemModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
