using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace BookManagement.ViewModels.Author
{
    /// <summary>
    /// Read-only display row combining a book's current approval status
    /// with the latest reviewer comment (if any has been left yet).
    /// </summary>
    public class AuthorApprovalItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SubmittedDate { get; set; } = string.Empty;
        public string LatestComment { get; set; } = string.Empty;
    }

    public class AuthorApprovalStatusViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;

        private const string AuthorName = "Alice Johnson";

        private string _searchText = string.Empty;
        private string _selectedStatus = "Tất cả";

        private int _pendingCount;
        private int _approvedCount;
        private int _rejectedCount;
        private int _draftCount;

        public ObservableCollection<AuthorApprovalItem> Items { get; } = new ObservableCollection<AuthorApprovalItem>();
        public ICollectionView ItemsView { get; }

        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string>
        {
            "Tất cả", "Pending", "Approved", "Rejected", "Draft"
        };

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
                }
            }
        }

        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }

        public int ApprovedCount
        {
            get => _approvedCount;
            set => SetProperty(ref _approvedCount, value);
        }

        public int RejectedCount
        {
            get => _rejectedCount;
            set => SetProperty(ref _rejectedCount, value);
        }

        public int DraftCount
        {
            get => _draftCount;
            set => SetProperty(ref _draftCount, value);
        }

        public AuthorApprovalStatusViewModel(DashboardViewModelBase dashboard, IBookService bookService, IReviewService reviewService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _reviewService = reviewService;

            LoadItems();

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterItems;
        }

        private void LoadItems()
        {
            Items.Clear();

            var myBooks = _bookService.GetMyBooks(1).Where(b => b.Author == AuthorName).ToList();
            var reviews = _reviewService.GetAllReviews().Where(r => r.AuthorName == AuthorName).ToList();

            foreach (var book in myBooks)
            {
                var latestComment = reviews
                    .Where(r => r.BookTitle == book.Title)
                    .OrderByDescending(r => r.Date)
                    .Select(r => r.AdminComment)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(latestComment))
                {
                    latestComment = book.Status == "Pending"
                        ? "Đang chờ Ban kiểm duyệt xem xét..."
                        : "Chưa có nhận xét";
                }

                Items.Add(new AuthorApprovalItem
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Category = book.Category,
                    Price = book.Price,
                    Status = book.Status,
                    SubmittedDate = book.SubmittedDate,
                    LatestComment = latestComment
                });
            }

            PendingCount = myBooks.Count(b => b.Status == "Pending");
            ApprovedCount = myBooks.Count(b => b.Status == "Approved");
            RejectedCount = myBooks.Count(b => b.Status == "Rejected");
            DraftCount = myBooks.Count(b => b.Status == "Draft");
        }

        private bool FilterItems(object obj)
        {
            if (obj is AuthorApprovalItem item)
            {
                if (SelectedStatus != "Tất cả" && item.Status != SelectedStatus)
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var text = SearchText.ToLower();
                    return item.Title.ToLower().Contains(text) || item.Category.ToLower().Contains(text);
                }

                return true;
            }
            return false;
        }
    }
}
