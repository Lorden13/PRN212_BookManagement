using System.Windows;
using BookManagement.Services;
using BookManagement.Views;

namespace BookManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register main navigation frame
            NavigationService.Instance.RegisterMainFrame(frmMain);
            
            // Start by navigating to the Login screen
            NavigationService.Instance.NavigateMain(new Login());
        }
    }
}