using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class PendingBooksViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public PendingBooksViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ApproveCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    MessageBox.Show($"Book \"{book.Title}\" has been approved!", "Book Approved", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            RejectCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    MessageBox.Show($"Book \"{book.Title}\" has been rejected!", "Book Rejected", MessageBoxButton.OK, MessageBoxImage.Warning);
            });

            LoadPendingBooks();
        }

        public ObservableCollection<Book> PendingBooks { get; } = new();

        public bool HasPendingBooks => PendingBooks.Count > 0;

        public RelayCommand ApproveCommand { get; }
        public RelayCommand RejectCommand { get; }

        private void LoadPendingBooks()
        {
            PendingBooks.Clear();
            var pending = MockDataService.Books.Where(b => b.Status == BookStatus.Pending);
            foreach (var book in pending)
                PendingBooks.Add(book);
            OnPropertyChanged(nameof(HasPendingBooks));
        }
    }
}
