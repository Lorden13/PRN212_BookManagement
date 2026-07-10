using System;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorCreateBookViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly BookViewModel? _existingBook;
        private readonly bool _isEditMode;

        private string _bookTitle = string.Empty;
        private string _bookCategory = string.Empty;
        private string _bookPriceString = string.Empty;
        private string _bookDescription = string.Empty;
        private string _bookCoverPath = string.Empty;
        private string _actionText = string.Empty;

        public string BookTitle
        {
            get => _bookTitle;
            set => SetProperty(ref _bookTitle, value);
        }

        public string BookCategory
        {
            get => _bookCategory;
            set => SetProperty(ref _bookCategory, value);
        }

        public string BookPrice
        {
            get => _bookPriceString;
            set => SetProperty(ref _bookPriceString, value);
        }

        public string BookDescription
        {
            get => _bookDescription;
            set => SetProperty(ref _bookDescription, value);
        }

        public string BookCoverPath
        {
            get => _bookCoverPath;
            set => SetProperty(ref _bookCoverPath, value);
        }

        public string ActionText
        {
            get => _actionText;
            set => SetProperty(ref _actionText, value);
        }

        public ICommand ActionCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand SelectCoverCommand { get; }

        // Create Mode
        public AuthorCreateBookViewModel(DashboardViewModelBase dashboard, IBookService bookService)
            : this(dashboard, bookService, null)
        {
        }

        // Edit Mode
        public AuthorCreateBookViewModel(DashboardViewModelBase dashboard, IBookService bookService, BookViewModel book)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _existingBook = book;
            _isEditMode = (book != null);

            ActionCommand = new RelayCommand(OnSubmit);
            CancelCommand = new RelayCommand(OnCancel);
            ResetCommand = new RelayCommand(OnReset);
            SelectCoverCommand = new RelayCommand(OnSelectCover);

            ResetFields();
        }

        private void ResetFields()
        {
            if (_isEditMode && _existingBook != null)
            {
                BookTitle = _existingBook.Title;
                BookCategory = _existingBook.Category;
                BookPrice = _existingBook.Price.ToString("F2");
                BookDescription = _existingBook.Description;
                BookCoverPath = _existingBook.CoverImagePath;
                ActionText = "Cập nhật";
            }
            else
            {
                BookTitle = "";
                BookCategory = "Programming";
                BookPrice = "0.00";
                BookDescription = "";
                BookCoverPath = "Chưa chọn ảnh bìa";
                ActionText = "Gửi yêu cầu";
            }
        }

        private void OnSubmit()
        {
            if (string.IsNullOrWhiteSpace(BookTitle))
            {
                _dashboard.ShowToast("Tiêu đề sách không được để trống!", "Warning");
                return;
            }

            if (BookTitle.Trim().Length < 3)
            {
                _dashboard.ShowToast("Tiêu đề sách phải có tối thiểu 3 ký tự!", "Warning");
                return;
            }

            if (string.IsNullOrWhiteSpace(BookDescription))
            {
                _dashboard.ShowToast("Mô tả sách không được để trống!", "Warning");
                return;
            }

            if (BookDescription.Trim().Length < 10)
            {
                _dashboard.ShowToast("Mô tả sách phải có tối thiểu 10 ký tự!", "Warning");
                return;
            }

            if (!double.TryParse(BookPrice, out double priceValue) || priceValue < 0)
            {
                _dashboard.ShowToast("Giá sách phải là số thực không âm!", "Warning");
                return;
            }

            try
            {
                if (_isEditMode && _existingBook != null)
                {
                    // Update existing
                    _existingBook.Title = BookTitle;
                    _existingBook.Category = BookCategory;
                    _existingBook.Price = priceValue;
                    _existingBook.Description = BookDescription;
                    _existingBook.CoverImagePath = BookCoverPath == "Chưa chọn ảnh bìa" ? "/Assets/Covers/placeholder.jpg" : BookCoverPath;
                    _existingBook.Status = "Pending"; // edit resubmits for approval

                    _bookService.UpdateBook(_existingBook.Model);

                    _dashboard.ShowToast($"Đã cập nhật sách '{BookTitle}' và gửi phê duyệt lại!", "Success");
                }
                else
                {
                    // Create new
                    var newBook = new BookModel
                    {
                        Title = BookTitle,
                        Category = BookCategory,
                        Price = priceValue,
                        Description = BookDescription,
                        Author = _dashboard.Sidebar.CurrentUser.Name, // use actual logged-in author name
                        CoverImagePath = BookCoverPath == "Chưa chọn ảnh bìa" ? "/Assets/Covers/placeholder.jpg" : BookCoverPath,
                        Status = "Pending",
                        Rating = 0.0,
                        SubmittedDate = DateTime.Now.ToString("yyyy-MM-dd")
                    };

                    _bookService.CreateBook(newBook);

                    _dashboard.ShowToast($"Đã tạo mới sách '{BookTitle}' và gửi phê duyệt thành công!", "Success");
                }
            }
            catch (Exception ex)
            {
                _dashboard.ShowToast($"Lỗi hệ thống: {ex.Message}", "Error");
                Console.WriteLine(ex.ToString());
                return; // Do not return to My Books page if the operation failed
            }

            // Return to My Books
            _dashboard.PageTitle = "My Books";
            _dashboard.CurrentPageViewModel = new AuthorBooksViewModel(_dashboard, _bookService);
        }

        private void OnCancel()
        {
            _dashboard.PageTitle = "My Books";
            _dashboard.CurrentPageViewModel = new AuthorBooksViewModel(_dashboard, _bookService);
        }

        private void OnReset()
        {
            ResetFields();
            _dashboard.ShowToast("Đã thiết lập lại biểu mẫu!", "Info");
        }

        private void OnSelectCover()
        {
            // Simulate cover selection dialog by picking a mock image path
            var mockCovers = new[] {
                "/Assets/Covers/custom_cover1.jpg",
                "/Assets/Covers/custom_cover2.jpg",
                "/Assets/Covers/custom_cover3.jpg"
            };
            
            var rand = new Random();
            BookCoverPath = mockCovers[rand.Next(mockCovers.Length)];
            _dashboard.ShowToast("Đã chọn ảnh bìa (UI Mock)!", "Info");
        }
    }
}
