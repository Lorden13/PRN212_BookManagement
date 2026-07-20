using System.Windows;
using System.Windows.Controls;


namespace BookManagement.WPF.Views.Reader
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private string _currentPassword = "";

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                _currentPassword = pb.Password;
            }
        }

        private string _newPassword = "";
        private string _confirmPassword = "";

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                _newPassword = pb.Password;
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                _confirmPassword = pb.Password;
        }
    }
}
