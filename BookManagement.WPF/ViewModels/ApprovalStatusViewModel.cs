using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ApprovalStatusItem
    {
        public Book Book { get; set; } = null!;
        public string Title => Book.Title;
        public string Category => Book.Category;
        public DateTime SubmittedDate => Book.CreatedDate;
        public BookStatus Status => Book.Status;
        public string LatestComment { get; set; } = string.Empty;
    }

    public class ApprovalStatusViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private bool _hasBooks;
        private string _searchText = string.Empty;
        private int _pendingCount;
        private int _approvedCount;
        private int _rejectedCount;

        public ApprovalStatusViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Items = new ObservableCollection<ApprovalStatusItem>();
            SearchCommand = new RelayCommand(_ => LoadItems());
            LoadItems();
        }

        public ObservableCollection<ApprovalStatusItem> Items { get; }

        public RelayCommand SearchCommand { get; }

        public bool HasBooks
        {
            get => _hasBooks;
            set => SetProperty(ref _hasBooks, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    LoadItems();
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

        private void LoadItems()
        {
            Items.Clear();

            var authorId = _mainViewModel.CurrentUser?.Id;
            var authorName = _mainViewModel.CurrentUser?.FullName ?? string.Empty;

            var books = MockDataService.Books
                .Where(b => b.AuthorId == authorId)
                .Where(b => string.IsNullOrWhiteSpace(SearchText) ||
                            b.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            PendingCount = books.Count(b => b.Status == BookStatus.Pending);
            ApprovedCount = books.Count(b => b.Status == BookStatus.Approved);
            RejectedCount = books.Count(b => b.Status == BookStatus.Rejected);

            foreach (var book in books)
            {
                var latestRecord = MockDataService.ApprovalRecords
                    .Where(r => r.BookTitle == book.Title &&
                                r.AuthorName.Equals(authorName, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                var comment = latestRecord == null
                    ? "Awaiting review."
                    : !string.IsNullOrWhiteSpace(latestRecord.RejectionReason)
                        ? latestRecord.RejectionReason!
                        : $"{latestRecord.Decision} by {latestRecord.ReviewedBy}.";

                Items.Add(new ApprovalStatusItem { Book = book, LatestComment = comment });
            }

            HasBooks = Items.Count > 0;
        }
    }
}
