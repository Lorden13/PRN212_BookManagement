using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorBooksViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private string _searchText = "";
        private string _selectedStatus = "Tất cả";

        public ObservableCollection<BookViewModel> MyBooks { get; } = new ObservableCollection<BookViewModel>();
        public ICollectionView MyBooksView { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    MyBooksView.Refresh();
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
                    MyBooksView.Refresh();
                }
            }
        }

        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string>
        {
            "Tất cả", "Approved", "Pending", "Rejected", "Draft"
        };

        public ICommand CreateBookCommand { get; }
        public ICommand ViewBookDetailCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public AuthorBooksViewModel(DashboardViewModelBase dashboard, IBookService bookService)
        {
            _dashboard = dashboard;
            _bookService = bookService;

            CreateBookCommand = new RelayCommand(OnCreateBook);
            ViewBookDetailCommand = new RelayCommand<BookViewModel>(OnViewBookDetail);
            EditBookCommand = new RelayCommand<BookViewModel>(OnEditBook, CanEditBook);
            DeleteBookCommand = new RelayCommand<BookViewModel>(OnDeleteBook, CanDeleteBook);

            LoadMyBooks();

            MyBooksView = CollectionViewSource.GetDefaultView(MyBooks);
            MyBooksView.Filter = FilterMyBooks;
        }

        private void LoadMyBooks()
        {
            MyBooks.Clear();
            var list = _bookService.GetMyBooks(1).Where(b => b.Author == _dashboard.Sidebar.CurrentUser.Name);
            foreach (var b in list)
            {
                var bookVM = new BookViewModel(b)
                {
                    SelectCommand = ViewBookDetailCommand
                };
                MyBooks.Add(bookVM);
            }
        }

        private bool FilterMyBooks(object obj)
        {
            if (obj is BookViewModel book)
            {
                // Status Filter
                if (SelectedStatus != "Tất cả" && book.Status != SelectedStatus)
                {
                    return false;
                }

                // Text Search Filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var text = SearchText.ToLower();
                    return book.Title.ToLower().Contains(text) || book.Category.ToLower().Contains(text);
                }

                return true;
            }
            return false;
        }

        private void OnCreateBook()
        {
            _dashboard.PageTitle = "Create Book";
            _dashboard.CurrentPageViewModel = new AuthorCreateBookViewModel(_dashboard, _bookService);
        }

        private void OnViewBookDetail(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.PageTitle = "Book Detail";
                _dashboard.CurrentPageViewModel = new AuthorBookDetailViewModel(_dashboard, _bookService, book);
            }
        }

        private bool CanEditBook(BookViewModel book)
        {
            // Edit is allowed for anything EXCEPT approved books
            return book != null && book.Status != "Approved";
        }

        private void OnEditBook(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.PageTitle = "Edit Book";
                _dashboard.CurrentPageViewModel = new AuthorCreateBookViewModel(_dashboard, _bookService, book);
            }
        }

        private bool CanDeleteBook(BookViewModel book)
        {
            // Delete is only allowed for Pending books
            return book != null && book.Status == "Pending";
        }

        private void OnDeleteBook(BookViewModel book)
        {
            if (book != null)
            {
                // Since there is a ConfirmationDialog in Controls, we can use it or just call a message box for simple yes/no.
                // Let's prompt a MessageBox for simple validation.
                var result = System.Windows.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa yêu cầu duyệt sách '{book.Title}'?", 
                    "Xác nhận xóa", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // To delete, we can either remove it from the mock list or update its status.
                    // Let's remove it from the MyBooks collection and mock database list.
                    // Wait, does MockBookService allow deletion? It doesn't have a Delete method in the interface,
                    // but we can update its status to "Draft" or mock delete by removing it from the MyBooks collection.
                    // Let's mock delete by removing it from MyBooks collection and showing toast.
                    MyBooks.Remove(book);
                    _dashboard.ShowToast($"Đã xóa sách '{book.Title}' thành công!", "Info");
                }
            }
        }
    }
}
