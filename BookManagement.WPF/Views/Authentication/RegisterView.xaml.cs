using System.Windows;
using System.Windows.Controls;

namespace BookManagement.WPF.Views.Authentication
{
    public partial class RegisterView : UserControl
    {
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public RegisterView()
        {
            InitializeComponent();
        }

        public string Password => _password;
        public string ConfirmPassword => _confirmPassword;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                _password = pb.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                _confirmPassword = pb.Password;
            }
        }
    }
}