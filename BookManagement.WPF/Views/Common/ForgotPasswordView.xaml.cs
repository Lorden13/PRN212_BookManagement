using System.Windows;
using System.Windows.Controls;
using BookManagement.ViewModels.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Common
{
    public partial class ForgotPasswordView : Page
    {
        public ForgotPasswordView()
        {
            InitializeComponent();
            var vm = App.Current.Services.GetRequiredService<ForgotPasswordViewModel>();
            DataContext = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ForgotPasswordViewModel.CurrentStep))
            {
                if (txtNewPassword != null) txtNewPassword.Password = string.Empty;
                if (txtConfirmPassword != null) txtConfirmPassword.Password = string.Empty;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel vm)
            {
                if (sender is PasswordBox passwordBox)
                {
                    if (passwordBox.Name == "txtNewPassword")
                    {
                        if (vm.NewPassword != passwordBox.Password)
                        {
                            vm.NewPassword = passwordBox.Password;
                        }
                    }
                    else if (passwordBox.Name == "txtConfirmPassword")
                    {
                        if (vm.ConfirmPassword != passwordBox.Password)
                        {
                            vm.ConfirmPassword = passwordBox.Password;
                        }
                    }
                }
            }
        }

        private void txtNewPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtNewPassword != null && txtNewPassword.Password != textBox.Text)
                {
                    txtNewPassword.Password = textBox.Text;
                }
            }
        }

        private void txtConfirmPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtConfirmPassword != null && txtConfirmPassword.Password != textBox.Text)
                {
                    txtConfirmPassword.Password = textBox.Text;
                }
            }
        }
    }
}
