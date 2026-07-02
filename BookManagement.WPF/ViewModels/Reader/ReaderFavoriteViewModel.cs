using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services.Mock;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderFavoriteViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IPurchaseService _purchaseService;
        private string _searchText = "";

        public ObservableCollection<BookViewModel> FavoriteBooks { get; } = new ObservableCollection<BookViewModel>();
        public ICollectionView FavoriteBooksView { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FavoriteBooksView.Refresh();
                }
            }
        }

        public ICommand RemoveFavoriteCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand BuyCommand { get; }

        public ReaderFavoriteViewModel(DashboardViewModelBase dashboard, IBookService bookService, IPurchaseService purchaseService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _purchaseService = purchaseService;

            RemoveFavoriteCommand = new RelayCommand<BookViewModel>(OnRemoveFavorite);
            ViewDetailCommand = new RelayCommand<BookViewModel>(OnViewDetail);
            BuyCommand = new RelayCommand<BookViewModel>(OnBuyBook);

            LoadFavorites();

            FavoriteBooksView = CollectionViewSource.GetDefaultView(FavoriteBooks);
            FavoriteBooksView.Filter = FilterFavorites;
        }

        private void LoadFavorites()
        {
            FavoriteBooks.Clear();
            var favoriteIds = MockReaderService.FavoriteBookIds;
            foreach (var id in favoriteIds)
            {
                var book = _bookService.GetBookById(id);
                if (book != null)
                {
                    var bookVM = new BookViewModel(book)
                    {
                        BuyCommand = BuyCommand,
                        SelectCommand = ViewDetailCommand
                    };
                    FavoriteBooks.Add(bookVM);
                }
            }
        }

        private bool FilterFavorites(object obj)
        {
            if (obj is BookViewModel book)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var text = SearchText.ToLower();
                    return book.Title.ToLower().Contains(text) || book.Author.ToLower().Contains(text);
                }
                return true;
            }
            return false;
        }

        private void OnRemoveFavorite(BookViewModel book)
        {
            if (book != null)
            {
                MockReaderService.FavoriteBookIds.Remove(book.Id);
                FavoriteBooks.Remove(book);
                _dashboard.ShowToast("Đã xóa khỏi danh sách yêu thích!", "Info");
            }
        }

        private void OnViewDetail(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.CurrentPageViewModel = new ReaderBookDetailViewModel(_dashboard, _bookService, _purchaseService, book);
            }
        }

        private void OnBuyBook(BookViewModel book)
        {
            if (book != null)
            {
                _purchaseService.PurchaseBook(1, book.Id);
                _dashboard.ShowToast($"Đã mua thành công sách: {book.Title}!", "Success");
            }
        }
    }
}
