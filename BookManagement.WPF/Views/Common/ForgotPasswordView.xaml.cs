using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Common
{
    public partial class ForgotPasswordView : Page
    {
        public ForgotPasswordView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<BookManagement.ViewModels.Common.ForgotPasswordViewModel>();
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
