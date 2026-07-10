using System.Windows.Controls;
using BookManagement.Models.Auth;
using BookManagement.Services.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Admin
{
    public partial class AdminDashboard : Page
    {
        private readonly AuthUtil _auth;
        public AdminDashboard()
        {
            InitializeComponent();
            _auth = new AuthUtil();
            this.IsVisibleChanged += Window_IsVisibleChanged;

        }
        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) 
            {
                CheckTokenResult checkedToken = await _auth.CheckTokenAsync();
                if (checkedToken != null && checkedToken.RoleName.Equals("Admin"))
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
