using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Common
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            var vm = App.Current.Services.GetRequiredService<LoginViewModel>();
            DataContext = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.IsRegisterMode))
            {
                if (txtPassword != null) txtPassword.Password = string.Empty;
                if (txtRegisterPassword != null) txtRegisterPassword.Password = string.Empty;
                if (txtRegisterConfirmPassword != null) txtRegisterConfirmPassword.Password = string.Empty;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                if (sender is PasswordBox passwordBox)
                {
                    if (passwordBox.Name == "txtPassword")
                    {
                        if (vm.Password != passwordBox.Password)
                        {
                            vm.Password = passwordBox.Password;
                        }
                    }
                    else if (passwordBox.Name == "txtRegisterPassword")
                    {
                        if (vm.RegisterPassword != passwordBox.Password)
                        {
                            vm.RegisterPassword = passwordBox.Password;
                        }
                    }
                    else if (passwordBox.Name == "txtRegisterConfirmPassword")
                    {
                        if (vm.RegisterConfirmPassword != passwordBox.Password)
                        {
                            vm.RegisterConfirmPassword = passwordBox.Password;
                        }
                    }
                }
            }
        }

        private void txtPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtPassword != null && txtPassword.Password != textBox.Text)
                {
                    txtPassword.Password = textBox.Text;
                }
            }
        }

        private void txtRegisterPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtRegisterPassword != null && txtRegisterPassword.Password != textBox.Text)
                {
                    txtRegisterPassword.Password = textBox.Text;
                }
            }
        }

        private void txtRegisterConfirmPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtRegisterConfirmPassword != null && txtRegisterConfirmPassword.Password != textBox.Text)
                {
                    txtRegisterConfirmPassword.Password = textBox.Text;
                }
            }
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
