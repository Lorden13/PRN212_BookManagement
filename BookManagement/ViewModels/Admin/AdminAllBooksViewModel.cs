using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminAllBooksViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IAuthorService _authorService;

        private string _searchText = string.Empty;
        private string _selectedCategory = "All";
        private string _selectedStatus = "All";

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

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    BooksView.Refresh();
                }
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    BooksView.Refresh();
                }
            }
        }

        public ObservableCollection<BookViewModel> Books { get; } = new ObservableCollection<BookViewModel>();
        public ICollectionView BooksView { get; }

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string> { "All", "Programming", "Novel", "Business", "Science", "History", "Education", "Technology" };
        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string> { "All", "Approved", "Pending", "Rejected", "Draft" };

        public ICommand ViewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public AdminAllBooksViewModel(
            DashboardViewModelBase dashboard, 
            IBookService bookService,
            IReviewService reviewService,
            IAuthorService authorService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _reviewService = reviewService;
            _authorService = authorService;

            ViewCommand = new RelayCommand<BookViewModel>(OnView);
            EditCommand = new RelayCommand<BookViewModel>(OnEdit);
            DeleteCommand = new RelayCommand<BookViewModel>(OnDelete);

            BooksView = CollectionViewSource.GetDefaultView(Books);
            BooksView.Filter = FilterBooks;

            LoadBooks();
        }

        private void LoadBooks()
        {
            Books.Clear();
            var allBooks = _bookService.GetApprovedBooks()
                .Concat(_bookService.GetPendingBooks())
                .Concat(new[] { _bookService.GetBookById(19), _bookService.GetBookById(22) })
                .Where(b => b != null)
                .Distinct()
                .ToList();

            foreach (var b in allBooks)
            {
                Books.Add(new BookViewModel(b));
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

            if (SelectedCategory != "All")
            {
                if (book.Category != SelectedCategory) return false;
            }

            if (SelectedStatus != "All")
            {
                if (book.Status != SelectedStatus) return false;
            }

            return true;
        }

        private void OnView(BookViewModel book)
        {
            if (book == null) return;
            _dashboard.PageTitle = "Book Review";
            _dashboard.CurrentPageViewModel = new AdminBookReviewViewModel(_dashboard, _bookService, _reviewService, _authorService, book);
        }

        private void OnEdit(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.ShowToast($"Chỉnh sửa thông tin sách: {book.Title} (Demo)", "Success");
            }
        }

        private void OnDelete(BookViewModel book)
        {
            if (book != null)
            {
                Books.Remove(book);
                _dashboard.ShowToast($"Đã xóa sách: {book.Title}", "Success");
            }
        }
    }
}
