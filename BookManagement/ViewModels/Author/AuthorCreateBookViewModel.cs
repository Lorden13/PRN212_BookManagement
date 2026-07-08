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
        private string _bookPdfPath = string.Empty;
        private string _actionText = string.Empty;

        // Inline validation error messages (empty string = no error)
        private string _titleError = string.Empty;
        private string _categoryError = string.Empty;
        private string _priceError = string.Empty;
        private string _descriptionError = string.Empty;
        private string _pdfError = string.Empty;

        public string BookTitle
        {
            get => _bookTitle;
            set
            {
                if (SetProperty(ref _bookTitle, value))
                {
                    ValidateTitle();
                }
            }
        }

        public string BookCategory
        {
            get => _bookCategory;
            set
            {
                if (SetProperty(ref _bookCategory, value))
                {
                    ValidateCategory();
                }
            }
        }

        public string BookPrice
        {
            get => _bookPriceString;
            set
            {
                if (SetProperty(ref _bookPriceString, value))
                {
                    ValidatePrice();
                }
            }
        }

        public string BookDescription
        {
            get => _bookDescription;
            set
            {
                if (SetProperty(ref _bookDescription, value))
                {
                    ValidateDescription();
                }
            }
        }

        public string BookCoverPath
        {
            get => _bookCoverPath;
            set => SetProperty(ref _bookCoverPath, value);
        }

        public string BookPdfPath
        {
            get => _bookPdfPath;
            set
            {
                if (SetProperty(ref _bookPdfPath, value))
                {
                    ValidatePdf();
                }
            }
        }

        public string ActionText
        {
            get => _actionText;
            set => SetProperty(ref _actionText, value);
        }

        public string TitleError
        {
            get => _titleError;
            set => SetProperty(ref _titleError, value);
        }

        public string CategoryError
        {
            get => _categoryError;
            set => SetProperty(ref _categoryError, value);
        }

        public string PriceError
        {
            get => _priceError;
            set => SetProperty(ref _priceError, value);
        }

        public string DescriptionError
        {
            get => _descriptionError;
            set => SetProperty(ref _descriptionError, value);
        }

        public string PdfError
        {
            get => _pdfError;
            set => SetProperty(ref _pdfError, value);
        }

        public ICommand ActionCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand SelectCoverCommand { get; }
        public ICommand SelectPdfCommand { get; }

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
            SelectPdfCommand = new RelayCommand(OnSelectPdf);

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
                BookPdfPath = string.IsNullOrWhiteSpace(_existingBook.PdfFilePath) ? "Chưa chọn tệp PDF" : _existingBook.PdfFilePath;
                ActionText = "Cập nhật";
            }
            else
            {
                BookTitle = "";
                BookCategory = "Programming";
                BookPrice = "0.00";
                BookDescription = "";
                BookCoverPath = "Chưa chọn ảnh bìa";
                BookPdfPath = "Chưa chọn tệp PDF";
                ActionText = "Gửi yêu cầu";
            }

            ClearErrors();
        }

        private void ClearErrors()
        {
            TitleError = string.Empty;
            CategoryError = string.Empty;
            PriceError = string.Empty;
            DescriptionError = string.Empty;
            PdfError = string.Empty;
        }

        // ---- Field-level (real-time) validation ----

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(BookTitle))
            {
                TitleError = "Tiêu đề sách không được để trống.";
                return false;
            }
            if (BookTitle.Trim().Length < 3)
            {
                TitleError = "Tiêu đề sách phải có ít nhất 3 ký tự.";
                return false;
            }
            TitleError = string.Empty;
            return true;
        }

        private bool ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(BookCategory))
            {
                CategoryError = "Vui lòng chọn thể loại.";
                return false;
            }
            CategoryError = string.Empty;
            return true;
        }

        private bool ValidatePrice()
        {
            if (string.IsNullOrWhiteSpace(BookPrice))
            {
                PriceError = "Giá sách không được để trống.";
                return false;
            }
            if (!double.TryParse(BookPrice, out double priceValue))
            {
                PriceError = "Giá sách phải là số hợp lệ.";
                return false;
            }
            if (priceValue < 0)
            {
                PriceError = "Giá sách không được là số âm.";
                return false;
            }
            if (priceValue > 1000)
            {
                PriceError = "Giá sách không được vượt quá $1000.";
                return false;
            }
            PriceError = string.Empty;
            return true;
        }

        private bool ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(BookDescription))
            {
                DescriptionError = "Mô tả sách không được để trống.";
                return false;
            }
            if (BookDescription.Trim().Length < 10)
            {
                DescriptionError = "Mô tả sách phải có ít nhất 10 ký tự.";
                return false;
            }
            DescriptionError = string.Empty;
            return true;
        }

        private bool ValidatePdf()
        {
            if (string.IsNullOrWhiteSpace(BookPdfPath) || BookPdfPath == "Chưa chọn tệp PDF")
            {
                PdfError = "Vui lòng tải lên tệp bản thảo PDF.";
                return false;
            }
            PdfError = string.Empty;
            return true;
        }

        private bool ValidateAll()
        {
            bool titleOk = ValidateTitle();
            bool categoryOk = ValidateCategory();
            bool priceOk = ValidatePrice();
            bool descriptionOk = ValidateDescription();
            bool pdfOk = ValidatePdf();

            return titleOk && categoryOk && priceOk && descriptionOk && pdfOk;
        }

        private void OnSubmit()
        {
            if (!ValidateAll())
            {
                _dashboard.ShowToast("Vui lòng kiểm tra lại các trường thông tin!", "Warning");
                return;
            }

            double priceValue = double.Parse(BookPrice);

            if (_isEditMode && _existingBook != null)
            {
                // Update existing
                _existingBook.Title = BookTitle;
                _existingBook.Category = BookCategory;
                _existingBook.Price = priceValue;
                _existingBook.Description = BookDescription;
                _existingBook.CoverImagePath = BookCoverPath == "Chưa chọn ảnh bìa" ? "/Assets/Covers/placeholder.jpg" : BookCoverPath;
                _existingBook.PdfFilePath = BookPdfPath;
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
                    Author = "Alice Johnson", // demo author
                    CoverImagePath = BookCoverPath == "Chưa chọn ảnh bìa" ? "/Assets/Covers/placeholder.jpg" : BookCoverPath,
                    PdfFilePath = BookPdfPath,
                    Status = "Pending",
                    Rating = 0.0,
                    SubmittedDate = DateTime.Now.ToString("yyyy-MM-dd")
                };

                _bookService.CreateBook(newBook);

                _dashboard.ShowToast($"Đã tạo mới sách '{BookTitle}' và gửi phê duyệt thành công!", "Success");
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

        private void OnSelectPdf()
        {
            // Simulate a PDF file picker dialog by picking a mock file path
            var mockPdfName = $"{(string.IsNullOrWhiteSpace(BookTitle) ? "ban_thao" : BookTitle.Replace(" ", "_").ToLower())}.pdf";
            BookPdfPath = $"/Assets/Manuscripts/{mockPdfName}";
            _dashboard.ShowToast("Đã tải lên tệp PDF (UI Mock)!", "Info");
        }
    }
}
