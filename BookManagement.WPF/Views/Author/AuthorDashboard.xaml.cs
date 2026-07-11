using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Author
{
    public partial class AuthorDashboard : Page
    {
        public AuthorDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<AuthorDashboardViewModel>();
        }
    }
}
