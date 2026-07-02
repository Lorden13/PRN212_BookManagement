using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminProfileView : UserControl
    {
        public AdminProfileView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminProfileViewModel vm)
            {
                vm.CurrentPassword = txtCurrentPassword.Password;
                vm.NewPassword = txtNewPassword.Password;
                vm.ConfirmPassword = txtConfirmPassword.Password;
            }
        }
    }
}
