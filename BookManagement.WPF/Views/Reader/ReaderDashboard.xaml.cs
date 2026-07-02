using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Reader
{
    public partial class ReaderDashboard : Page
    {
        public ReaderDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<ReaderDashboardViewModel>();
        }
    }
}
