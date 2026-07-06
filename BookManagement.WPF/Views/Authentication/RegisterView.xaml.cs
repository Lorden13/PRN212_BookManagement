using System.Windows;
using System.Windows.Controls;
using BookManagement.WPF.ViewModels;

namespace BookManagement.WPF.Views.Authentication
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.SetPassword(((PasswordBox)sender).Password);
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.SetConfirmPassword(((PasswordBox)sender).Password);
        }
    }
}
