using System.Windows;
using System.Windows.Controls;
using LoginViewModel = BookManagement.WPF.ViewModels.LoginViewModel;

namespace BookManagement.WPF.Views.Authentication
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.SetPassword(((PasswordBox)sender).Password);
            }
        }
    }
}
