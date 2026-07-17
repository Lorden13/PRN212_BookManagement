using System.Windows;
using BookManagement.Services;
using BookManagement.Views;

namespace BookManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Console.WriteLine("[MainWindow] Constructor called");
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"[MainWindow] NavigationService type: {typeof(NavigationService).FullName}");
            // Register main navigation frame
            NavigationService.Instance.RegisterMainFrame(frmMain);
            
            // Start by navigating to the Login screen
            NavigationService.Instance.NavigateMain(new Login());
        }
    }
}