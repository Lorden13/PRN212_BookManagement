using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Admin
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<AdminDashboardViewModel>();
        }
    }
}
