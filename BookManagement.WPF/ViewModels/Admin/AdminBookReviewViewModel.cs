using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminBookReviewViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IAuthorService _authorService;
        private string _adminComment = string.Empty;

        public BookViewModel Book { get; }

        public string AdminComment
        {
            get => _adminComment;
            set => SetProperty(ref _adminComment, value);
        }

        public ObservableCollection<ReviewModel> ReviewHistory { get; } = new ObservableCollection<ReviewModel>();

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand BackCommand { get; }

        public AdminBookReviewViewModel(
            DashboardViewModelBase dashboard,
            IBookService bookService,
            IReviewService reviewService,
            IAuthorService authorService,
            BookViewModel book)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _reviewService = reviewService;
            _authorService = authorService;
            Book = book;

            ApproveCommand = new RelayCommand(OnApprove);
            RejectCommand = new RelayCommand(OnReject);
            BackCommand = new RelayCommand(OnBack);

            LoadReviewHistory();
        }

        private void LoadReviewHistory()
        {
            ReviewHistory.Clear();
            var reviews = _reviewService.GetReviewsByBookId(Book.Id).ToList();
            foreach (var r in reviews)
            {
                ReviewHistory.Add(r);
            }
        }

        private void OnApprove()
        {
            _bookService.ApproveBook(Book.Id);
            _reviewService.SubmitReview(new ReviewModel
            {
                BookId = Book.Id,
                BookTitle = Book.Title,
                AuthorName = Book.Author,
                Result = "Approved",
                AdminComment = string.IsNullOrEmpty(AdminComment) ? "Tác phẩm đã được phê duyệt." : AdminComment,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
            _dashboard.ShowToast($"Đã phê duyệt sách: {Book.Title}", "Success");
            OnBack();
        }

        private void OnReject()
        {
            if (string.IsNullOrEmpty(AdminComment))
            {
                _dashboard.ShowToast("Vui lòng nhập lý do từ chối vào ô nhận xét!", "Warning");
                return;
            }

            _bookService.RejectBook(Book.Id, AdminComment);
            _reviewService.SubmitReview(new ReviewModel
            {
                BookId = Book.Id,
                BookTitle = Book.Title,
                AuthorName = Book.Author,
                Result = "Rejected",
                AdminComment = AdminComment,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
            _dashboard.ShowToast($"Đã từ chối sách: {Book.Title}", "Danger");
            OnBack();
        }

        private void OnBack()
        {
            _dashboard.PageTitle = "Pending Books";
            _dashboard.CurrentPageViewModel = new AdminPendingBooksViewModel(_dashboard, _bookService, _authorService, _reviewService);
        }
    }
}
