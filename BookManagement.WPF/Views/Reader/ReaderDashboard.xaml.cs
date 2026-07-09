using BookManagement.Models.Auth;
using BookManagement.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace BookManagement.Views.Reader
{
    public partial class ReaderDashboard : Page
    {
        private readonly AuthUtil _auth;
        public ReaderDashboard()
        {
            _auth = new AuthUtil();
            InitializeComponent();
            this.IsVisibleChanged += Window_IsVisibleChanged;
        }
        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CheckTokenResult checkedToken = await _auth.CheckTokenAsync();
                if (checkedToken != null && checkedToken.RoleName.Equals("Reader"))
                {
                    return;
                }
                else Services.Navigation.NavigationService.Instance.NavigateMain(new Login());

            }
            else
            {
            }
        }

    }
}
