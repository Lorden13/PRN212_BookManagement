using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class FavoriteBooksViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public FavoriteBooksViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            RemoveFavoriteCommand = new RelayCommand(OnRemoveFavorite);
            ViewDetailsCommand = new RelayCommand(OnViewDetails);
            LoadData();
        }

        public ObservableCollection<Book> FavoriteBooks { get; } = new();

        public RelayCommand RemoveFavoriteCommand { get; }
        public RelayCommand ViewDetailsCommand { get; }

        private bool _hasFavorites;
        public bool HasFavorites
        {
            get => _hasFavorites;
            set => SetProperty(ref _hasFavorites, value);
        }

        private void LoadData()
        {
            var currentUserId = _mainViewModel.CurrentUser?.Id ?? 3;
            var favoriteBookIds = MockDataService.FavoriteBooks
                .Where(f => f.UserId == currentUserId)
                .Select(f => f.BookId)
                .ToList();
            var books = MockDataService.Books
                .Where(b => favoriteBookIds.Contains(b.Id))
                .ToList();

            FavoriteBooks.Clear();
            foreach (var book in books)
                FavoriteBooks.Add(book);

            HasFavorites = FavoriteBooks.Count > 0;
        }

        private void OnRemoveFavorite(object? parameter)
        {
            if (parameter is Book book)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove '{book.Title}' from your favorites?",
                    "Remove Favorite",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    FavoriteBooks.Remove(book);
                    HasFavorites = FavoriteBooks.Count > 0;
                    MessageBox.Show($"'{book.Title}' has been removed from your favorites.", "Removed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void OnViewDetails(object? parameter)
        {
            if (parameter is Book book)
            {
                _mainViewModel.NavigateToBookDetails(book);
            }
        }
    }
}
