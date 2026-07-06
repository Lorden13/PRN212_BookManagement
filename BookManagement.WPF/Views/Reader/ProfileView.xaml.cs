using System.Windows;
using System.Windows.Controls;
using BookManagement.WPF.ViewModels;

namespace BookManagement.WPF.Views.Reader
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
            {
                vm.CurrentPassword = pb.Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
            {
                vm.NewPassword = pb.Password;
            }
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
            {
                vm.ConfirmNewPassword = pb.Password;
            }
        }
    }
}
