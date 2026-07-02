using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminPendingBooksViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IReviewService _reviewService;

        private string _searchText = string.Empty;
        private string _selectedAuthor = "All";

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    BooksView.Refresh();
                }
            }
        }

        public string SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (SetProperty(ref _selectedAuthor, value))
                {
                    BooksView.Refresh();
                }
            }
        }

        public ObservableCollection<BookViewModel> Books { get; } = new ObservableCollection<BookViewModel>();
        public ICollectionView BooksView { get; }

        public ObservableCollection<string> Authors { get; } = new ObservableCollection<string>();

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand ViewCommand { get; }

        public AdminPendingBooksViewModel(
            DashboardViewModelBase dashboard,
            IBookService bookService,
            IAuthorService authorService,
            IReviewService reviewService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _authorService = authorService;
            _reviewService = reviewService;

            ApproveCommand = new RelayCommand<BookViewModel>(OnApprove);
            RejectCommand = new RelayCommand<BookViewModel>(OnReject);
            ViewCommand = new RelayCommand<BookViewModel>(OnView);

            BooksView = CollectionViewSource.GetDefaultView(Books);
            BooksView.Filter = FilterBooks;

            LoadPendingBooks();
        }

        private void LoadPendingBooks()
        {
            Books.Clear();
            var pendingList = _bookService.GetPendingBooks().ToList();
            foreach (var b in pendingList)
            {
                Books.Add(new BookViewModel(b));
            }

            Authors.Clear();
            Authors.Add("All");
            var authorNames = pendingList.Select(b => b.Author).Distinct().ToList();
            foreach (var name in authorNames)
            {
                Authors.Add(name);
            }
        }

        private bool FilterBooks(object item)
        {
            if (!(item is BookViewModel book)) return false;

            if (!string.IsNullOrEmpty(SearchText))
            {
                bool matchesSearch = book.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                     book.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                if (!matchesSearch) return false;
            }

            if (SelectedAuthor != "All")
            {
                if (book.Author != SelectedAuthor) return false;
            }

            return true;
        }

        private void OnApprove(BookViewModel book)
        {
            if (book == null) return;
            _bookService.ApproveBook(book.Id);
            _reviewService.SubmitReview(new ReviewModel
            {
                BookId = book.Id,
                BookTitle = book.Title,
                AuthorName = book.Author,
                Result = "Approved",
                AdminComment = "Tác phẩm được phê duyệt tự động bởi quản trị viên.",
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
            _dashboard.ShowToast($"Đã phê duyệt sách: {book.Title}", "Success");
            LoadPendingBooks();
        }

        private void OnReject(BookViewModel book)
        {
            if (book == null) return;
            OnView(book);
        }

        private void OnView(BookViewModel book)
        {
            if (book == null) return;
            _dashboard.PageTitle = "Book Review";
            _dashboard.CurrentPageViewModel = new AdminBookReviewViewModel(_dashboard, _bookService, _reviewService, _authorService, book);
        }
    }
}
