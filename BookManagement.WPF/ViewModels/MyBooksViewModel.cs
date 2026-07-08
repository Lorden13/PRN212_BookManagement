using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class MyBooksViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _searchText = string.Empty;
        private bool _hasBooks;

        public MyBooksViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            MyBooks = new ObservableCollection<Book>();
            SearchCommand = new RelayCommand(_ => Search());
            AddBookCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("AddBook"));
            ViewCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    MessageBox.Show($"Viewing: {book.Title}\n\nCategory: {book.Category}\nPrice: ${book.Price:F2}\nStatus: {book.Status}\n\n{book.Description}", "Book Details", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            EditCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    _mainViewModel.NavigateToEditBook(book);
            });
            DeleteCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete \"{book.Title}\"?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                        MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
            LoadBooks();
        }

        public ObservableCollection<Book> MyBooks { get; }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool HasBooks
        {
            get => _hasBooks;
            set => SetProperty(ref _hasBooks, value);
        }

        public RelayCommand SearchCommand { get; }
        public RelayCommand AddBookCommand { get; }
        public RelayCommand ViewCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        private void LoadBooks()
        {
            MyBooks.Clear();
            var books = MockDataService.Books
                .Where(b => b.AuthorId == _mainViewModel.CurrentUser?.Id)
                .ToList();

            foreach (var book in books)
                MyBooks.Add(book);

            HasBooks = MyBooks.Count > 0;
        }

        private void Search()
        {
            MyBooks.Clear();
            var books = MockDataService.Books
                .Where(b => b.AuthorId == _mainViewModel.CurrentUser?.Id)
                .Where(b => string.IsNullOrWhiteSpace(SearchText) ||
                            b.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            foreach (var book in books)
                MyBooks.Add(book);

            HasBooks = MyBooks.Count > 0;
        }
    }
}
