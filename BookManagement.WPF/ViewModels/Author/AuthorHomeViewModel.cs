using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorHomeViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;

        private int _totalBooks;
        private int _pendingBooks;
        private int _approvedBooks;
        private int _rejectedBooks;

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

        public int ApprovedBooks
        {
            get => _approvedBooks;
            set => SetProperty(ref _approvedBooks, value);
        }

        public int RejectedBooks
        {
            get => _rejectedBooks;
            set => SetProperty(ref _rejectedBooks, value);
        }

        public ObservableCollection<BookViewModel> RecentBooks { get; } = new ObservableCollection<BookViewModel>();

        public ICommand CreateBookCommand { get; }
        public ICommand ViewBookCommand { get; }

        public AuthorHomeViewModel(DashboardViewModelBase dashboard, IBookService bookService)
        {
            _dashboard = dashboard;
            _bookService = bookService;

            CreateBookCommand = new RelayCommand(OnCreateBook);
            ViewBookCommand = new RelayCommand<BookViewModel>(OnViewBook);

            LoadStatistics();
        }

        private void LoadStatistics()
        {
            // Demo author is Alice Johnson (Id = 1). Let's filter by Author Name for demo.
            var myBooks = _bookService.GetMyBooks(1).Where(b => b.Author == _dashboard.Sidebar.CurrentUser.Name).ToList();

            TotalBooks = myBooks.Count;
            PendingBooks = myBooks.Count(b => b.Status == "Pending");
            ApprovedBooks = myBooks.Count(b => b.Status == "Approved");
            RejectedBooks = myBooks.Count(b => b.Status == "Rejected");

            RecentBooks.Clear();
            foreach (var b in myBooks.Take(5))
            {
                RecentBooks.Add(new BookViewModel(b)
                {
                    SelectCommand = ViewBookCommand
                });
            }
        }

        private void OnCreateBook()
        {
            _dashboard.PageTitle = "Create Book";
            _dashboard.CurrentPageViewModel = new AuthorCreateBookViewModel(_dashboard, _bookService);
        }

        private void OnViewBook(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.PageTitle = "Book Detail";
                _dashboard.CurrentPageViewModel = new AuthorBookDetailViewModel(_dashboard, _bookService, book);
            }
        }
    }
}
