using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminUsersView : UserControl
    {
        public AdminUsersView()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateAdminWindow createWindow = new CreateAdminWindow();
            createWindow.Show();
        }
    }
}
