using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class BookDetailsViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public BookDetailsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            FavoriteCommand = new RelayCommand(_ => OnFavorite());
            BuyCommand = new RelayCommand(_ => OnBuy());
            BackCommand = new RelayCommand(_ => OnBack());
        }

        public BookDetailsViewModel(MainViewModel mainViewModel, Book book) : this(mainViewModel)
        {
            SelectedBook = book;
            Title = book.Title;
            AuthorName = book.AuthorName;
            Description = book.Description;
            Category = book.Category;
            Price = book.Price.ToString("C");
            CoverColor = book.CoverColor;
        }

        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _authorName = string.Empty;
        public string AuthorName
        {
            get => _authorName;
            set => SetProperty(ref _authorName, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _category = string.Empty;
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string _price = string.Empty;
        public string Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private string _coverColor = "#2563EB";
        public string CoverColor
        {
            get => _coverColor;
            set => SetProperty(ref _coverColor, value);
        }

        public RelayCommand FavoriteCommand { get; }
        public RelayCommand BuyCommand { get; }
        public RelayCommand BackCommand { get; }

        private void OnFavorite()
        {
            if (SelectedBook != null)
            {
                MessageBox.Show($"'{SelectedBook.Title}' has been added to your favorites!", "Added to Favorites", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnBuy()
        {
            if (SelectedBook != null)
            {
                MessageBox.Show($"Purchase successful! You bought '{SelectedBook.Title}' for {SelectedBook.Price:C}.", "Purchase Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnBack()
        {
            _mainViewModel.CurrentViewModel = new BrowseBooksViewModel(_mainViewModel);
        }
    }
}
