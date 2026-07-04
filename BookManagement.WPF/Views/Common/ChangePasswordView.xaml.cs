using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Common
{
    public partial class ChangePasswordView : UserControl
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm)
            {
                vm.CurrentPassword = txtCurrentPassword.Password;
                vm.NewPassword = txtNewPassword.Password;
                vm.ConfirmPassword = txtConfirmPassword.Password;
            }
        }
    }
}
