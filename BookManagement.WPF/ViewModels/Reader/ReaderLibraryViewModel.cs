using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderLibraryViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IPurchaseService _purchaseService;
        private readonly IBookService _bookService;
        private string _searchText = string.Empty;

        public ObservableCollection<LibraryItemViewModel> LibraryItems { get; } = new ObservableCollection<LibraryItemViewModel>();
        public ICollectionView LibraryItemsView { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LibraryItemsView.Refresh();
                }
            }
        }

        public ICommand ReadCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand ViewBookDetailCommand { get; }

        public ReaderLibraryViewModel(DashboardViewModelBase dashboard, IPurchaseService purchaseService, IBookService bookService)
        {
            _dashboard = dashboard;
            _purchaseService = purchaseService;
            _bookService = bookService;

            ReadCommand = new RelayCommand<LibraryItemViewModel>(OnRead);
            DownloadCommand = new RelayCommand<LibraryItemViewModel>(OnDownload);
            ViewBookDetailCommand = new RelayCommand<LibraryItemViewModel>(OnViewBookDetail);

            LoadLibrary();

            LibraryItemsView = CollectionViewSource.GetDefaultView(LibraryItems);
            LibraryItemsView.Filter = FilterLibrary;
        }

        private void LoadLibrary()
        {
            LibraryItems.Clear();
            // Demo reader ID = 1
            var history = _purchaseService.GetPurchaseHistory(1);
            foreach (var p in history)
            {
                var book = _bookService.GetBookById(p.BookId);
                var libraryItem = new LibraryItemViewModel(p, book);
                LibraryItems.Add(libraryItem);
            }
        }

        private bool FilterLibrary(object obj)
        {
            if (obj is LibraryItemViewModel item)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var text = SearchText.ToLower();
                    return item.BookTitle.ToLower().Contains(text) || 
                           item.Author.ToLower().Contains(text) ||
                           item.Status.ToLower().Contains(text);
                }
                return true;
            }
            return false;
        }

        private void OnRead(LibraryItemViewModel item)
        {
            if (item != null)
            {
                _dashboard.ShowToast($"Reading book: '{item.BookTitle}'...", "Success");
            }
        }

        private async void OnDownload(LibraryItemViewModel item)
        {
            if (item == null) return;

            if (item.IsDownloaded)
            {
                _dashboard.ShowToast($"Sách '{item.BookTitle}' đã được tải xuống điện tử thành công!", "Info");
                return;
            }

            if (item.IsDownloading) return;

            item.IsDownloading = true;
            item.DownloadStatus = "Downloading 0%";
            item.DownloadProgress = 0;
            _dashboard.ShowToast($"Đang kết nối để tải sách: {item.BookTitle}", "Info");

            // Simulate progress bar updates
            int[] progressStages = { 20, 50, 85, 100 };
            foreach (var progress in progressStages)
            {
                await System.Threading.Tasks.Task.Delay(250);
                item.DownloadProgress = progress;
                item.DownloadStatus = $"Downloading {progress}%";
            }

            item.IsDownloading = false;
            item.IsDownloaded = true;
            item.DownloadStatus = "Downloaded";
            _dashboard.ShowToast($"Tải thành công sách: {item.BookTitle}!", "Success");
        }

        private void OnViewBookDetail(LibraryItemViewModel item)
        {
            if (item != null && item.Book != null)
            {
                var bookVM = new BookViewModel(item.Book)
                {
                    BuyCommand = new RelayCommand<BookViewModel>((bk) => 
                    {
                        _purchaseService.PurchaseBook(1, bk.Id);
                        _dashboard.ShowToast($"Đã mua thành công sách: {bk.Title}!", "Success");
                    })
                };
                _dashboard.CurrentPageViewModel = new ReaderBookDetailViewModel(_dashboard, _bookService, _purchaseService, bookVM);
            }
        }
    }

    public class LibraryItemViewModel : BaseViewModel
    {
        private bool _isDownloaded;
        private bool _isDownloading;
        private int _downloadProgress;
        private string _downloadStatus = "Not Downloaded";

        public PurchaseModel Purchase { get; }
        public BookModel Book { get; }

        public int Id => Purchase.Id;
        public string BookTitle => Purchase.BookTitle;
        public string PurchaseDate => Purchase.PurchaseDate;
        public string Status => Purchase.Status;
        public string Author => Book?.Author ?? "Unknown Author";
        public string CoverImagePath => Book?.CoverImagePath ?? string.Empty;

        public bool IsDownloaded
        {
            get => _isDownloaded;
            set => SetProperty(ref _isDownloaded, value);
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set => SetProperty(ref _isDownloading, value);
        }

        public int DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        public string DownloadStatus
        {
            get => _downloadStatus;
            set => SetProperty(ref _downloadStatus, value);
        }

        public LibraryItemViewModel(PurchaseModel purchase, BookModel book)
        {
            Purchase = purchase;
            Book = book;
            
            // Seed initial state: first few items downloaded, others not
            if (purchase.Id % 3 == 0)
            {
                IsDownloaded = true;
                DownloadStatus = "Downloaded";
                DownloadProgress = 100;
            }
        }
    }
}
