using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services.Mock;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderBookDetailViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IPurchaseService _purchaseService;
        private readonly IReviewService? _reviewService;

        public BookViewModel Book { get; }
        public ObservableCollection<BookViewModel> RecommendedBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<ReviewModel> Reviews { get; } = new ObservableCollection<ReviewModel>();

        public ICommand BuyCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand BackCommand { get; }

        public ReaderBookDetailViewModel(
            DashboardViewModelBase dashboard, 
            IBookService bookService, 
            IPurchaseService purchaseService, 
            BookViewModel book)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _purchaseService = purchaseService;
            _reviewService = App.Current.Services.GetService(typeof(IReviewService)) as IReviewService;
            Book = book;

            BuyCommand = new RelayCommand(OnBuyBook);
            AddToFavoritesCommand = new RelayCommand(OnAddToFavorites);
            BackCommand = new RelayCommand(OnBack);

            LoadRecommendedBooks();
            LoadReviews();
        }

        private void LoadRecommendedBooks()
        {
            RecommendedBooks.Clear();
            var related = _bookService.GetApprovedBooks()
                .Where(b => b.Category == Book.Category && b.Id != Book.Id)
                .Take(3);

            foreach (var b in related)
            {
                var bookVM = new BookViewModel(b)
                {
                    BuyCommand = new RelayCommand<BookViewModel>((bk) => 
                    {
                        _purchaseService.PurchaseBook(1, bk.Id);
                        _dashboard.ShowToast($"Đã mua thành công sách: {bk.Title}!", "Success");
                    }),
                    SelectCommand = new RelayCommand<BookViewModel>((bk) => 
                    {
                        // Navigate to detail of recommended book
                        _dashboard.CurrentPageViewModel = new ReaderBookDetailViewModel(_dashboard, _bookService, _purchaseService, bk);
                    })
                };
                RecommendedBooks.Add(bookVM);
            }
        }

        private void LoadReviews()
        {
            Reviews.Clear();
            if (_reviewService != null)
            {
                var bookReviews = _reviewService.GetReviewsByBookId(Book.Id);
                foreach (var r in bookReviews)
                {
                    Reviews.Add(r);
                }
            }
        }

        private void OnBuyBook()
        {
            _purchaseService.PurchaseBook(1, Book.Id);
            _dashboard.ShowToast($"Đã mua thành công sách: {Book.Title}!", "Success");
        }

        private void OnAddToFavorites()
        {
            if (MockReaderService.FavoriteBookIds.Contains(Book.Id))
            {
                _dashboard.ShowToast("Sách này đã có trong danh sách yêu thích!", "Warning");
            }
            else
            {
                MockReaderService.FavoriteBookIds.Add(Book.Id);
                _dashboard.ShowToast("Đã thêm vào danh sách yêu thích!", "Success");
            }
        }

        private void OnBack()
        {
            _dashboard.PageTitle = "Books";
            _dashboard.CurrentPageViewModel = new ReaderBooksViewModel(_dashboard, _bookService, _purchaseService);
        }
    }
}
