using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class BrowseBooksViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private readonly List<Book> _allBooks;

        public BrowseBooksViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            _allBooks = MockDataService.Books
                .Where(b => b.Status == BookStatus.Approved)
                .ToList();

            // Build categories list
            var cats = _allBooks.Select(b => b.Category).Distinct().OrderBy(c => c).ToList();
            cats.Insert(0, "All");
            Categories = cats;
            _selectedCategory = "All";

            SearchCommand = new RelayCommand(_ => ApplyFilter());
            ViewDetailsCommand = new RelayCommand(OnViewDetails);
            FavoriteCommand = new RelayCommand(OnFavorite);
            BuyCommand = new RelayCommand(OnBuy);

            ApplyFilter();
        }

        public ObservableCollection<Book> Books { get; } = new();
        public List<string> Categories { get; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    ApplyFilter();
            }
        }

        private bool _hasBooks;
        public bool HasBooks
        {
            get => _hasBooks;
            set => SetProperty(ref _hasBooks, value);
        }

        public RelayCommand SearchCommand { get; }
        public RelayCommand ViewDetailsCommand { get; }
        public RelayCommand FavoriteCommand { get; }
        public RelayCommand BuyCommand { get; }

        private void ApplyFilter()
        {
            var filtered = _allBooks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.Trim();
                filtered = filtered.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            {
                filtered = filtered.Where(b => b.Category == SelectedCategory);
            }

            Books.Clear();
            foreach (var book in filtered)
                Books.Add(book);

            HasBooks = Books.Count > 0;
        }

        private void OnViewDetails(object? parameter)
        {
            if (parameter is Book book)
            {
                _mainViewModel.NavigateToBookDetails(book);
            }
        }

        private void OnFavorite(object? parameter)
        {
            if (parameter is Book book)
            {
                MessageBox.Show($"'{book.Title}' has been added to your favorites!", "Added to Favorites", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnBuy(object? parameter)
        {
            if (parameter is Book book)
            {
                MessageBox.Show($"Purchase successful! You bought '{book.Title}' for {book.Price:C}.", "Purchase Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
