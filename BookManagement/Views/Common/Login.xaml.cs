using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Common
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<LoginViewModel>();
        }
    }
}
