using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ApprovalStatusViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private bool _hasBooks;

        public ApprovalStatusViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Books = new ObservableCollection<Book>();
            LoadBooks();
        }

        public ObservableCollection<Book> Books { get; }

        public bool HasBooks
        {
            get => _hasBooks;
            set => SetProperty(ref _hasBooks, value);
        }

        private void LoadBooks()
        {
            Books.Clear();
            var books = MockDataService.Books
                .Where(b => b.AuthorId == _mainViewModel.CurrentUser?.Id)
                .ToList();

            foreach (var book in books)
                Books.Add(book);

            HasBooks = Books.Count > 0;
        }
    }
}
