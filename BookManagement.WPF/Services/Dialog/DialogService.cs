using System.Windows;

namespace BookManagement.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
