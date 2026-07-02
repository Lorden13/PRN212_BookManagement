using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminHomeViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;
        private readonly IPurchaseService _purchaseService;
        private readonly IReviewService _reviewService;

        private int _totalUsers;
        private int _totalReaders;
        private int _totalAuthors;
        private int _totalBooks;
        private int _pendingBooks;
        private double _totalRevenue;
        private double _todaySales;

        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        public int TotalReaders
        {
            get => _totalReaders;
            set => SetProperty(ref _totalReaders, value);
        }

        public int TotalAuthors
        {
            get => _totalAuthors;
            set => SetProperty(ref _totalAuthors, value);
        }

        public int TotalBooks
        {
            get => _totalBooks;
            set => SetProperty(ref _totalBooks, value);
        }

        public int PendingBooks
        {
            get => _pendingBooks;
            set => SetProperty(ref _pendingBooks, value);
        }

        public double TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public double TodaySales
        {
            get => _todaySales;
            set => SetProperty(ref _todaySales, value);
        }

        public ObservableCollection<BookViewModel> LatestPendingBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<BookViewModel> TopSellingBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<ActivityLogModel> RecentActivities { get; } = new ObservableCollection<ActivityLogModel>();

        public ICommand ViewBookCommand { get; }
        public ICommand ViewUserCommand { get; }


        public AdminHomeViewModel(
            DashboardViewModelBase dashboard,
            IBookService bookService,
            IReaderService readerService,
            IAuthorService authorService,
            IPurchaseService purchaseService,
            IReviewService reviewService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _readerService = readerService;
            _authorService = authorService;
            _purchaseService = purchaseService;
            _reviewService = reviewService;

            ViewBookCommand = new RelayCommand<BookViewModel>(OnViewBook);
            ViewUserCommand = new RelayCommand<BookViewModel>(OnViewBook); // Just a fallback or custom commands

            LoadData();
        }

        private void LoadData()
        {
            var readers = _readerService.GetAllReaders().ToList();
            var authors = _authorService.GetAllAuthors().ToList();
            var pendingBooks = _bookService.GetPendingBooks().ToList();
            var allBooks = _bookService.GetApprovedBooks().Concat(_bookService.GetPendingBooks()).ToList();
            var purchases = _purchaseService.GetAllPurchases().ToList();

            TotalReaders = readers.Count;
            TotalAuthors = authors.Count;
            TotalUsers = TotalReaders + TotalAuthors + 1; // +1 for Admin
            TotalBooks = allBooks.Count;
            PendingBooks = pendingBooks.Count;

            TotalRevenue = purchases.Where(p => p.Status == "Completed").Sum(p => p.Price);
            TodaySales = purchases.Where(p => p.PurchaseDate == "2026-05-20").Sum(p => p.Price);

            // Latest Pending Books
            LatestPendingBooks.Clear();
            foreach (var b in pendingBooks.OrderByDescending(b => b.SubmittedDate).Take(5))
            {
                LatestPendingBooks.Add(new BookViewModel(b) { SelectCommand = ViewBookCommand });
            }

            // Top Selling Books
            TopSellingBooks.Clear();
            var bookSales = purchases
                .Where(p => p.Status == "Completed")
                .GroupBy(p => p.BookId)
                .Select(g => new { BookId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            foreach (var s in bookSales)
            {
                var book = allBooks.FirstOrDefault(b => b.Id == s.BookId);
                if (book != null)
                {
                    TopSellingBooks.Add(new BookViewModel(book) { SelectCommand = ViewBookCommand });
                }
            }

            // Recent Activities
            RecentActivities.Clear();
            RecentActivities.Add(new ActivityLogModel { Message = "John Smith purchased 'Clean Code'", Time = "10 mins ago", Type = "Purchase" });
            RecentActivities.Add(new ActivityLogModel { Message = "Alice Johnson submitted 'WPF Deep Dive' for review", Time = "30 mins ago", Type = "Submit" });
            RecentActivities.Add(new ActivityLogModel { Message = "Emma Watson registered as a new Reader", Time = "1 hour ago", Type = "Register" });
            RecentActivities.Add(new ActivityLogModel { Message = "Martin Fowler submitted 'Refactoring Patterns'", Time = "2 hours ago", Type = "Submit" });
            RecentActivities.Add(new ActivityLogModel { Message = "William Wilson purchased 'Design Patterns'", Time = "4 hours ago", Type = "Purchase" });
        }

        private void OnViewBook(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.PageTitle = "Book Review";
                _dashboard.CurrentPageViewModel = new AdminBookReviewViewModel(_dashboard, _bookService, _reviewService, _authorService, book);
            }
        }
    }
}
