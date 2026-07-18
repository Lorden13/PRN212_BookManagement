using System;
using System.Windows;
using System.Windows.Controls;
using BookManagement.WPF.Services.AccountService;
using BookManagement.Services.Utils;

namespace BookManagement.Views.Common
{
    public partial class ChangePasswordView : UserControl
    {
        private readonly AccountService _accountService;

        public ChangePasswordView()
        {
            // ReaderBookDetailView.xaml.cs = new AccountService();

            InitializeComponent();
            _accountService = new AccountService();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Optional: Can be left empty or used for validation
        }

        private async void BtnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtCurrentPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(currentPassword) || 
                string.IsNullOrWhiteSpace(newPassword) || 
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải dài từ 6 ký tự trở lên!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không trùng khớp!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var currentUser = UserSession.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("Lỗi phiên đăng nhập. Vui lòng đăng nhập lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool isSuccess = await _accountService.UpdatePassword(currentUser.AccountId, currentPassword, newPassword);

                if (isSuccess)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Đổi mật khẩu thất bại. Mật khẩu hiện tại không chính xác!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
