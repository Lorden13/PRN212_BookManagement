using BookManagement.Services.Utils;
using BookManagement.WPF.Entities;
using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Common
{
    public partial class UserProfileView : UserControl
    {
        public UserProfileView()
        {
            InitializeComponent();

            Loaded += UserProfileView_Loaded;
        }

        private void UserProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user != null)
            {
                txtName.Text = user.FullName ?? string.Empty;
                txtEmail.Text = user.Email ?? string.Empty;
                txtPhone.Text = user.Phone ?? string.Empty;
                txtRoleBadge.Text = !string.IsNullOrEmpty(user.Role) ? user.Role : "User";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            // 1. Validate FullName
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validate Email Required & Format
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmailFormat(email))
            {
                MessageBox.Show("Định dạng email không hợp lệ. Vui lòng kiểm tra lại.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new ProjectPrnContext())
                {
                    // 3. Validate Email Uniqueness (Problem 2 Fix)
                    bool isEmailDuplicate = db.Accounts.Any(a => a.Email == email && a.AccountId != user.AccountId);
                    if (isEmailDuplicate)
                    {
                        MessageBox.Show("Địa chỉ Gmail này đã được đăng ký bởi tài khoản khác trong hệ thống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 4. Update Database Account
                    var account = db.Accounts.FirstOrDefault(a => a.AccountId == user.AccountId);
                    if (account != null)
                    {
                        account.FullName = name;
                        account.Email = email;
                        account.Phone = phone;

                        db.Accounts.Update(account);
                        db.SaveChanges();
                    }
                }

                // 5. Synchronize UserSession
                user.FullName = name;
                user.Email = email;
                user.Phone = phone;

                MessageBox.Show("Cập nhật thông tin cá nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
