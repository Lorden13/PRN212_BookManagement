using System.Windows;
using System.Windows.Controls;
using BookManagement.WPF.ViewModels;

namespace BookManagement.WPF.Views.Common
{
    public partial class ChangePasswordView : UserControl
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm && sender is PasswordBox pb)
            {
                vm.CurrentPassword = pb.Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm && sender is PasswordBox pb)
            {
                vm.NewPassword = pb.Password;
            }
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm && sender is PasswordBox pb)
            {
                vm.ConfirmNewPassword = pb.Password;
            }
        }
    }
}
