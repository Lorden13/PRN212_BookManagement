using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ReaderDashboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public ReaderDashboardViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ViewDetailsCommand = new RelayCommand(OnViewDetails);
            LoadData();
        }

        public ObservableCollection<Book> RecentBooks { get; } = new();
        public ObservableCollection<Book> RecommendedBooks { get; } = new();
        public ObservableCollection<Book> FavoriteBooksList { get; } = new();

        public RelayCommand ViewDetailsCommand { get; }

        private bool _hasRecent;
        public bool HasRecent
        {
            get => _hasRecent;
            set => SetProperty(ref _hasRecent, value);
        }

        private bool _hasRecommended;
        public bool HasRecommended
        {
            get => _hasRecommended;
            set => SetProperty(ref _hasRecommended, value);
        }

        private bool _hasFavorites;
        public bool HasFavorites
        {
            get => _hasFavorites;
            set => SetProperty(ref _hasFavorites, value);
        }

        private void LoadData()
        {
            var approvedBooks = MockDataService.Books
                .Where(b => b.Status == BookStatus.Approved)
                .ToList();

            // Recent books: last 4 by CreatedDate
            var recent = approvedBooks
                .OrderByDescending(b => b.CreatedDate)
                .Take(4)
                .ToList();
            RecentBooks.Clear();
            foreach (var book in recent) RecentBooks.Add(book);
            HasRecent = RecentBooks.Count > 0;

            // Recommended books: 4 random approved books
            var random = new Random();
            var recommended = approvedBooks
                .OrderBy(_ => random.Next())
                .Take(4)
                .ToList();
            RecommendedBooks.Clear();
            foreach (var book in recommended) RecommendedBooks.Add(book);
            HasRecommended = RecommendedBooks.Count > 0;

            // Favorite books for current reader
            var currentUserId = _mainViewModel.CurrentUser?.Id ?? 3;
            var favoriteBookIds = MockDataService.FavoriteBooks
                .Where(f => f.UserId == currentUserId)
                .Select(f => f.BookId)
                .ToList();
            var favoriteBooks = MockDataService.Books
                .Where(b => favoriteBookIds.Contains(b.Id))
                .ToList();
            FavoriteBooksList.Clear();
            foreach (var book in favoriteBooks) FavoriteBooksList.Add(book);
            HasFavorites = FavoriteBooksList.Count > 0;
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
